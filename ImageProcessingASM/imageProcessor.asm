.code

; variables:
    ; r14d - y - outer loop variable
    ; r13d - x - inner loop variable
    ; r12 - sumB - accumulator for blue channel
    ; r15 - sumG - accumulator for green channel
    ; rsi - sumR - accumulator for red channel
    ; rdi - sumA - accumulator for alpha channel
    ; rbx - count - number of pixels in the block
    ; r10d - dy - block row iteration variable
    ; r11d - dx - block column iteration variable


PixelizeImage PROC

    ; Parameters (in x64 Windows with MSVC calling convention):
    ; RCX -> imageData (pointer to the image pixel data)
    ; RDX -> width      (int)
    ; R8  -> height     (int)
    ; R9  -> pixelSize  (int)

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

    ; 3) Initialize outer loop variable y -> r14d = 0

    xor r14d, r14d      ; r14d holds y, set to 0

OuterLoopY: ; Pętla iteruje po rzędach (y) obrazu, aż do osiągnięcia wysokości obrazu (height).
    ; Compare current y with height (r8d)

    cmp r14d, r8d       ; y >= height?
    jge EndLoopY        ; if so, exit outer loop

    ; 4) Initialize x for inner loop -> r13d = 0

    xor r13d, r13d      ; r13d holds x, set to 0

OuterLoopX: ; Pętla iteruje po kolumnach (x) obrazu, aż do osiągnięcia szerokości obrazu (width).
    ; Compare current x with width (edx)

    cmp r13d, edx       ; x >= width?
    jge EndLoopX        ; if so, exit inner loop

    ; 5) Initialize accumulators for this block and set them to 0:
    ;    sumB -> r12
    ;    sumG -> r15
    ;    sumR -> rsi
    ;    sumA -> rdi
    ;    count -> ebx

    xor r12, r12
    xor r15, r15
    xor rsi, rsi
    xor rdi, rdi
    xor ebx, ebx

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
    cmp eax, edx         ; compare currentX with width
    jge SkipPixel        ; if out of width, skip

    lea rbx, [r14 + r10] ; currentY
    cmp ebx, r8d         ; compare currentY with height
    jge SkipPixel        ; if out of height, skip

    ; 8) Calculate pixel index in a 32-bit RGBA format:
    ;    index = (currentY * width + currentX) * 4

    imul rbx, rdx        ; rbx = currentY * width
    add rbx, rax         ; rbx += currentX
    shl rbx, 2           ; multiply by 4 (shift left by 2)

    ; 9) Read the 4 channels (B, G, R, A) from the image memory and accumulate

    movzx eax, byte ptr [rcx + rbx]      ; B
    add r12, rax

    movzx eax, byte ptr [rcx + rbx + 1]  ; G
    add r15, rax

    movzx eax, byte ptr [rcx + rbx + 2]  ; R
    add rsi, rax

    movzx eax, byte ptr [rcx + rbx + 3]  ; A
    add rdi, rax

    inc ebx             ; count++

SkipPixel:
    inc r11d            ; dx++
    jmp BlockLoopX

EndBlockX:
    inc r10d            ; dy++
    jmp BlockLoopY

EndBlockY:
    ; 10) Check if we accumulated any pixels

    test ebx, ebx       ; check if count == 0
    jz SkipBlock        ; if count = 0, skip the block color assignment

    ; 11) Compute the average B, G, R, A
    ;     We use 8-bit for each average, so we divide sum by count and store AL

    xor rax, rax
    mov eax, r12d       ; sumB
    cdq                 ; sign extend
    idiv ebx            ; divide by count
    movzx r12d, al      ; r12d = avgB

    xor rax, rax
    mov eax, r15d       ; sumG
    cdq
    idiv ebx
    movzx r15d, al      ; r15d = avgG

    xor rax, rax
    mov eax, esi        ; sumR
    cdq
    idiv ebx
    movzx rsi, al       ; rsi = avgR

    xor rax, rax
    mov eax, edi        ; sumA
    cdq
    idiv ebx
    movzx rdi, al       ; rdi = avgA

SkipBlock:
    inc r13d            ; x++
    jmp OuterLoopX

EndLoopX:
    inc r14d            ; y++
    jmp OuterLoopY

EndLoopY:
    ; 12) Restore registers

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

END

The program in asm should:
1. Get the image data, width, height, and pixel size as parameters.
2. Iterate over the image pixels in blocks of pixelSize x pixelSize.
3. For each block, calculate the average color (B, G, R, A) and assign it to all pixels in the block.
4. Return the modified image data.
