.code

; Variables:
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
    ; ymm0 - 16-bit values for two pixels
    ; ymm1 - accumulator for 16-bit values
    ; xmm3 - 16-bit values for two pixels

; Parameters:
    ; RCX -> imageData (pointer to the image pixel data)
    ; RDX -> width      (int)
    ; R8  -> height     (int)
    ; R9  -> pixelSize  (int)

PixelizeImage PROC
    ; Check if pixelSize < 1, if so exit immediately
    cmp r9d, 1          ; Compare pixelSize to 1
    jl Exit             ; Jump to Exit if pixelSize < 1

    ; Save registers we will use
    push rbx
    push rsi
    push rdi
    push r12
    push r13
    push r14
    push r15
    push rbp

    ; Initialize outer loop variable y -> r14d = 0
    xor r14d, r14d      ; r14d holds y, set to 0

    ; Set count value to pixelSize^2 for calculating average pixel value
    mov ebp, r9d
    imul rbp, rbp

OuterLoopY: ; Loop iterates over rows (y) adding pixelSize every iteration of the image until height is reached
    ; Compare current y with height (r8d)
    cmp r14d, r8d       ; y >= height?
    jge EndLoopY        ; if so, exit outer loop

    ; Initialize x for inner loop -> r13d = 0
    xor r13d, r13d      ; r13d holds x, set to 0

OuterLoopX: ; Loop iterates over columns (x) adding pixelSize every iteration of the image until width is reached
    ; Compare current x with width (edx)
    cmp r13d, edx       ; x >= width?
    jge EndLoopX        ; if so, exit inner loop

    ; Initialize accumulators for this block and set them to 0:
    xor r12, r12 ; sumB
    xor r15, r15 ; sumG
    xor rsi, rsi ; sumR
    xor rdi, rdi ; sumA

    ; Clear accumulator for 2 pixels
    vpxor ymm1, ymm1, ymm1            

    ; Loop variables for block iteration:
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

    ; Compute currentX = x + dx, currentY = y + dy
    lea rax, [r13 + r11] ; currentX
    cmp rax, rdx         ; compare currentX with width
    jge SkipPixel        ; if out of width, skip

    lea rbx, [r14 + r10] ; currentY
    cmp rbx, r8          ; compare currentY with height
    jge SkipPixel        ; if out of height, skip

    ; Calculate pixel index in a 32-bit RGBA format:
    ; index = (currentY * width + currentX) * 4
    imul rbx, rdx        ; rbx = currentY * width
    add rbx, rax         ; rbx += currentX
    shl rbx, 2           ; multiply by 4 (shift left by 2)

    ; 9) Read two pixels from the image data and unpack them to 16-bit values   
    ;!!!!!!!!!!!!!!!!!! error with the following line, when crashes always when trying to read 8th byte !!!!!!!!!!!!!!!!!
    movdqu xmm0, [rcx + rbx]        ; BRGA; 
    vpmovzxbd ymm0, xmm0            ; Unpack 8-bit to 16-bit, ymm2 = [B0 G0 R0 A0 | B1 G1 R1 A1]
    vpaddd ymm1, ymm1, ymm0         ; Accumulate 16-bit values, ymm1 += ymm2


SkipPixel:
    add r11d, 2         ; we are processing 2 pixels at a time
    jmp BlockLoopX

EndBlockX:
    inc r10d            ; dy++
    jmp BlockLoopY

EndBlockY:
    ; Calculate the average of the 2 pixels in the block
    vmovdqa ymm4, ymm1            ; Copy ymm1 to ymm4
    vextracti128 xmm1, ymm1, 1    ; Extract the upper 128 bits of ymm1 into xmm1 | First pixel
    vextracti128 xmm4, ymm4, 0    ; Extract the lower 128 bits of ymm3 into xmm4 | Second pixel

    ; First pixel
    pextrd r12d, xmm4, 0         ; B
    pextrd r15d, xmm4, 1         ; G
    pextrd esi, xmm4, 2          ; R
    pextrd edi, xmm4, 3          ; A

    ; Second pixel + First pixel
    pextrd eax, xmm1, 0         ; B
    add r12d, eax
    pextrd eax, xmm1, 1         ; G
    add r15d, eax
    pextrd eax, xmm1, 2         ; R
    add esi, eax
    pextrd eax, xmm1, 3         ; A
    add edi, eax

    push rdx            ; Save rdx register because cdq will overwrite it

    ; Calculate the average value of RGBA in the block
    xor rax, rax        ; Clear rax
    mov eax, r12d       ; sumB
    cdq                 ; sign extend
    idiv rbp            ; divide by count
    movzx r12d, al      ; r12d = avgB

    xor rax, rax        ; Clear rax
    mov eax, r15d       ; sumG
    cdq
    idiv rbp
    movzx r15d, al      ; r15d = avgG

    xor rax, rax        ; Clear rax
    mov eax, esi        ; sumR
    cdq
    idiv rbp
    movzx rsi, al       ; rsi = avgR

    xor rax, rax        ; Clear rax
    mov eax, edi        ; sumA
    cdq
    idiv rbp
    movzx rdi, al       ; rdi = avgA

    pop rdx             ; Restore rdx register 

    ;Clear xmm1 register to insert the average value
    vpxor xmm1, xmm1, xmm1

    ; Insert four registers to xmm1
    pinsrb xmm1, r12d, 0         ; Insert avgB into xmm1[0]
    pinsrb xmm1, r15d, 1         ; Insert avgG into xmm1[1]
    pinsrb xmm1, esi, 2          ; Insert avgR into xmm1[2]
    pinsrb xmm1, edi, 3          ; Insert avgA into xmm1[3]
    
    ; Broadcast the average value to all 16-bit values in ymm1
    ; So that we can insert the average value 4 pixels at a time
    pshufd xmm1, xmm1, 0

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

    ; Write the average to 4 pixels in the block
    movdqu [rcx + rbx], xmm1

SkipWrite:
    add r11d, 4         ; we are inserting 4 pixels at a time
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
    ; Restore registers
    pop rbp
    pop r15
    pop r14
    pop r13
    pop r12
    pop rdi
    pop rsi
    pop rbx

Exit:
    ; Return to caller
    ret
PixelizeImage ENDP
end