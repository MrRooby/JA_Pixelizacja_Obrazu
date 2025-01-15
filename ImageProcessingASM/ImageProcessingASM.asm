.code

; Variables:
    ; r14 - y - outer loop variable
    ; r13 - x - inner loop variable
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
    push r13
    push r14    
    push rbp

    ; Initialize outer loop variable y -> r14d = 0
    xor r14d, r14d      ; r14d holds y, set to 0

    ; Set count value to pixelSize^2 for calculating average pixel value
    ; mov ebp, r9d
    ; imul rbp, rbp

    ; We will be dividing accumulator which will contain RGBA values
    ; so we move the count value to xmm4 for division
    vmovd xmm4, ebp             ; Move 32-bit count value to xmm4
    pshufd xmm4, xmm4, 0        ; Broadcast the 32-bit value to all 4 bytes
    cvtdq2ps xmm4, xmm4         ; Convert 32-bit int to float for division

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
    vpxor ymm1, ymm1, ymm1    ; Clear accumulator for 2 pixels       

    ; Initialize block pixel counter
    xor rbp, rbp        ; rbp holds count, set to 0 

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
    movdqu xmm0, [rcx + rbx]        ; BRGA; 
    vpmovzxbd ymm0, xmm0            ; Unpack 8-bit to 32-bit, ymm2 = [B0 G0 R0 A0 | B1 G1 R1 A1]
    vpaddd ymm1, ymm1, ymm0         ; Accumulate 32-bit values, ymm1 += ymm2

    add rbp, 2          ; Increment pixel counter by 2

SkipPixel:
    add r11d, 2         ; dx += 2 because we are processing 2 pixels at a time
    jmp BlockLoopX

EndBlockX:
    inc r10d            ; dy++
    jmp BlockLoopY

EndBlockY:
    
	; Check if we have processed any pixels in the block
    test rbp, rbp       ; if count == 0, skip block
    jz SkipBlock

    ; Pack pixel count to 16-bit values for division
    movd xmm4, ebp              ; Move 32-bit count value to xmm3
    pshufd xmm4, xmm4, 0        ; Broadcast the 32-bit value to all 4 bytes
    cvtdq2ps xmm4, xmm4         ; Convert 32-bit int to float for division

    ; Calculate the average RGBA value for the block
    vmovdqa ymm5, ymm1            ; Copy ymm1 to ymm4 for adding up two pixels from accumulator
    vextracti128 xmm1, ymm1, 1    ; Extract the upper 128 bits of ymm1 into xmm1 | First pixel
    vextracti128 xmm5, ymm5, 0    ; Extract the lower 128 bits of ymm3 into xmm4 | Second pixel

    paddd xmm1, xmm5              ; Add both accumulator values
    cvtdq2ps xmm1, xmm1           ; Change values from int to float for division
    divps xmm1, xmm4              ; Divide accumulated data by pixel count of the block
    cvttps2dq xmm1, xmm1          ; Change values back to int

    ; Pack the 32-bit values in xmm1 to 8-bit values for insertion
    packusdw xmm1, xmm1           ; Pack the 32-bit values in xmm1 to 16-bit values
    packuswb xmm1, xmm1           ; Pack the 16-bit values in xmm1 to 8-bit values

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
    pop r14
    pop r13
    pop rbx

Exit:
    ; Return to caller
    ret
PixelizeImage ENDP
end