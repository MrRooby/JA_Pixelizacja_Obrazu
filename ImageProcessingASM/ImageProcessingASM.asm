.code

; variables:
    ; r14 - y - outer loop variable
    ; r13 - x - inner loop variable
    ; r12 - sumB - accumulator for blue channel
    ; r15 - sumG - accumulator for green channel
    ; rsi - sumR - accumulator for red channel
    ; rdi - sumA - accumulator for alpha channel
    ; r10 - dy - block row iteration variable
    ; r11 - dx - block column iteration variable
    ; rbp - count - number of pixels in the block
    ; rbx - currentY - current block row
    ; rax - currentX - current block column

; Parameters:
    ; RCX -> imageData (pointer to the image pixel data)
    ; RDX -> width      (int)
    ; R8  -> height     (int)
    ; R9  -> pixelSize  (int)

PixelizeImage PROC
    ; 1) Check if pixelSize < 1, if so exit immediately
    cmp r9d, 1          ; Compare pixelSize to 1
    jl Exit             ; Jump to Exit if pixelSize < 1

    ; 2) Save registers we will use
    push rbx
    push rsi
    push rdi
    push r12
    push r13
    push r14
    push r15
    push rbp

    ; 3) Initialize outer loop variable y -> r14d = 0
    xor r14d, r14d      ; r14d holds y, set to 0

    ;set count value to pixelSize^2 for calculating average
    mov ebp, r9d
    imul rbp, rbp

OuterLoopY: ; Loop iterates over rows (y) adding pixelSize every iteration of the image until height is reached
    ; Compare current y with height (r8d)
    cmp r14d, r8d       ; y >= height?
    jge EndLoopY        ; if so, exit outer loop

    ; 4) Initialize x for inner loop -> r13d = 0
    xor r13d, r13d      ; r13d holds x, set to 0

OuterLoopX: ; Loop iterates over columns (x) adding pixelSize every iteration of the image until width is reached
    ; Compare current x with width (edx)

    cmp r13d, edx       ; x >= width?
    jge EndLoopX        ; if so, exit inner loop

    ; 5) Initialize accumulators for this block and set them to 0:
    ;    sumB -> r12
    ;    sumG -> r15
    ;    sumR -> rsi
    ;    sumA -> rdi

    xor r12, r12
    xor r15, r15
    xor rsi, rsi
    xor rdi, rdi

    ; 6) Loop variables for block iteration:
    ;    dy = r10d
    ;    dx = r11d

    mov r10d, 0         ; dy = 0

BlockLoopY:
    ; If dy >= pixelSize -> done with block row

    cmp r10d, r9d
    jge EndBlockY

    ; Start with dx = 0

    mov r11d, 0         ; dx = 0

BlockLoopX:
    ; If dx >= pixelSize -> done with block column
    cmp r11d, r9d
    jge EndBlockX

    ; 7) Compute currentX = x + dx, currentY = y + dy

    lea rax, [r13 + r11] ; currentX
    cmp rax, rdx         ; compare currentX with width
    jge SkipPixel        ; if out of width, skip

    lea rbx, [r14 + r10] ; currentY
    cmp rbx, r8          ; compare currentY with height
    jge SkipPixel        ; if out of height, skip

    ; 8) Calculate pixel index in a 32-bit RGBA format:
    ;    index = (currentY * width + currentX) * 4

    imul rbx, rdx        ; rbx = currentY * width
    add rbx, rax         ; rbx += currentX
    shl rbx, 2           ; multiply by 4 (shift left by 2)

    ; 9) Read the 4 channels (B, G, R, A) from the image memory and accumulate

    movzx rax, byte ptr [rcx + rbx]      ; B
    add r12, rax

    movzx rax, byte ptr [rcx + rbx + 1]  ; G
    add r15, rax

    movzx rax, byte ptr [rcx + rbx + 2]  ; R
    add rsi, rax

    movzx rax, byte ptr [rcx + rbx + 3]  ; A
    add rdi, rax

SkipPixel:
    inc r11d            ; dx++
    jmp BlockLoopX

EndBlockX:
    inc r10d            ; dy++
    jmp BlockLoopY

EndBlockY:
    ; 11) Compute the average B, G, R, A

    ; We use 8-bit for each average, so we divide sum by count and store AL

    push rdx ; saving rdx register because cdq instruction overrides it and we need it later

    xor rax, rax
    mov eax, r12d       ; sumB
    cdq                 ; sign extend
    idiv rbp            ; divide by count
    movzx r12d, al      ; r12d = avgB

    xor rax, rax
    mov eax, r15d       ; sumG
    cdq
    idiv rbp
    movzx r15d, al      ; r15d = avgG

    xor rax, rax
    mov eax, esi        ; sumR
    cdq
    idiv rbp
    movzx rsi, al       ; rsi = avgR

    xor rax, rax
    mov eax, edi        ; sumA
    cdq
    idiv rbp
    movzx rdi, al       ; rdi = avgA

    pop rdx ; restoring rdx register after average calculation

    ; Write the average to all pixels in the block
    mov r10d, 0         ; dy = 0

WriteBlockY:
    cmp r10d, r9d       ; if dy >= pixelSize, exit write loop
    jge EndWriteY

    mov r11d, 0         ; dx = 0

WriteBlockX:
    cmp r11d, r9d       ; if dx >= pixelSize, exit write loop
    jge EndWriteX

    ; Compute currentX = x + dx, currentY = y + dy
    lea rax, [r13 + r11] ; currentX
    cmp eax, edx         ; currentX >= width?
    jge SkipWrite

    lea rbx, [r14 + r10] ; currentY
    cmp ebx, r8d         ; currentY >= height?
    jge SkipWrite

    ; Calculate pixel index in 32-bit RGBA format
    imul rbx, rdx        ; rbx = currentY * width
    add rbx, rax         ; rbx += currentX
    shl rbx, 2           ; multiply by 4 (shift left by 2)

    ; Write average values back to the pixel
    mov byte ptr [rcx + rbx], r12b      ; B
    mov byte ptr [rcx + rbx + 1], r15b  ; G
    mov byte ptr [rcx + rbx + 2], sil   ; R
    mov byte ptr [rcx + rbx + 3], dil   ; A

SkipWrite:
    inc r11d            ; dx++
    jmp WriteBlockX

EndWriteX:
    inc r10d            ; dy++
    jmp WriteBlockY

EndWriteY:
    ; Continue to the next block in the outer loop
    add r13d, r9d       ; x += pixelSize
    jmp OuterLoopX      ; repeat for the next column

SkipBlock:
    add r13d, r9d       ; x += pixelSize
    jmp OuterLoopX

EndLoopX:
    add r14d, r9d       ; y += pixelSize
    jmp OuterLoopY

EndLoopY:
    ; 12) Restore registers
    pop rbp
    pop r15
    pop r14
    pop r13
    pop r12
    pop rdi
    pop rsi
    pop rbx

Exit:
    ; 13) Return to caller
    ret
PixelizeImage ENDP
end