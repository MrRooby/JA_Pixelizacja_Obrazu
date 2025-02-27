;-------------------------------------------------------------------------------------------;
;-------------------------------------------------------------------------------------------;
;  _____ _______   ________ _      _____ ____________   _____ __  __          _____ ______  ;
; |  __ \_   _\ \ / /  ____| |    |_   _|___  /  ____| |_   _|  \/  |   /\   / ____|  ____| ;
; | |__) || |  \ V /| |__  | |      | |    / /| |__      | | | \  / |  /  \ | |  __| |__    ;
; |  ___/ | |   > < |  __| | |      | |   / / |  __|     | | | |\/| | / /\ \| | |_ |  __|   ;
; | |    _| |_ / . \| |____| |____ _| |_ / /__| |____   _| |_| |  | |/ ____ \ |__| | |____  ;
; |_|   |_____/_/ \_\______|______|_____/_____|______| |_____|_|  |_/_/    \_\_____|______| ;
;-------------------------------------------------------------------------------------------;
;                                  SIMD AVX VERSION                                         ;                
;-------------------------------------------------------------------------------------------;
; Program: PixelizeImage                                                                    ;
; Version: 1.0                                                                              ;
; Author: Bartosz Faruga                                                                    ;
; Description: This procedure pixelizes an image by averaging the pixel values              ;
;              within blocks of a specified size. The image is processed in                 ;
;              blocks of size pixelSize x pixelSize. For each block, the                    ;
;              average color value is computed and assigned to all pixels                   ;
;              within that block. This results in a pixelated effect where                  ;
;              details are reduced and the image appears as a mosaic of                     ;
;              larger colored blocks.                                                       ;
; Parameters:                                                                               ;
;  RCX -> imageData ; POINTER to the image 32-bit RGBA pixel data                           ;
;  RDX -> width     ; INTEGER Width of the image in pixels                                  ; 
;                   ; !MUST BE DIVISIBLE BY 4, THE PROGRAM DOESN'T CHECK!                   ;
;  R8  -> height    ; INTEGER Height of the image in pixels                                 ;
;                   ; !MUST BE DIVISIBLE BY 4, THE PROGRAM DOESN'T CHECK!                   ;
;  R9  -> pixelSize ; INTEGER Size of the pixel block for averaging                         ;
;                   ; !MUST BE A POWER OF 4 EXCLUDING 1,                                    ;
;                   ; THE PROGRAM DOESN'T CHECK!                                            ;
; Error Handling:                                                                           ;
;   - If pixelSize < 1, the procedure exits immediately.                                    ;
;   - Ensure that width and height are positive integers.                                   ;
;   - Ensure that imageData is a valid pointer to the image data.                           ;
;                                                                                           ;
; ASSUMPTIONS (program does not check if true):                                             ;
;   - The image data is stored in 32-bit RGBA format.                                       ;
;   - The pixelSize is divisible by 4.                                                      ;
;   - The width and height are divisible by 4.                                              ;
;   - AVX instructions are supported                                                        ;
;                                                                                           ;
; Technologies Used:                                                                        ;
;   - Assembly language (x86-64)                                                            ;
;   - SIMD (Single Instruction, Multiple Data) instructions                                 ;
;   - AVX (Advanced Vector Extensions) instructions for parallel processing                 ;
;-------------------------------------------------------------------------------------------;
;-------------------------------------------------------------------------------------------;

.code
PixelizeImage PROC
    ; Check if pixelSize < 1, if so exit immediately
    cmp r9d, 1                  ; Compare pixelSize to 1
    jl Exit                     ; Jump to Exit if pixelSize < 1

    ; Save registers that will be used throughout the procedure
    ; so we can restore them before exiting
    push rbx
    push r13
    push r14    
    push rbp

    ; Initialize outer loop variable (block row) y -> r14d = 0
    xor r14d, r14d              ; r14d holds y, set to 0

OuterLoopY: ; Loop iterates over rows (y) by adding pixelSize every iteration of the image until height is reached
    ; Compare current y with height (r8d)
    cmp r14d, r8d               ; have we reached the end of the image?
    jge EndLoopY                ; if so, exit outer loop

    ; Initialize x for inner loop -> r13d = 0
    xor r13d, r13d              ; r13d holds x, set to 0

OuterLoopX: ; Loop iterates over columns (x) adding pixelSize every iteration of the image until width is reached
    ; Compare current x with width (edx)
    cmp r13d, edx               ; have we reached the end of the row?
    jge EndLoopX                ; if so, exit inner loop

    ; Initialize accumulators for this block and set them to 0:
    vpxor ymm1, ymm1, ymm1      ; Clear register for accumulating values for two pixels at a time 

    ; Initialize the counter for the number of pixels accumulated in the block
    xor rbp, rbp                ; rbp holds count, set to 0 

    ; Loop variables for block iteration:
    ;    dy = r10d - row offset in the block
    ;    dx = r11d - column offset in the block
    mov r10d, 0                 ; dy = 0

BlockLoopY:
    ; Check if we have reached the end of the block
    cmp r10d, r9d               ; if dy >= pixelSize
    jge EndBlockY               ; if so, exit block loop

    ; Start with dx = 0
    mov r11d, 0                 ; dx = 0

BlockLoopX:
    ; Check if we have reached the end of the block
    cmp r11d, r9d               ; if dx >= pixelSize
    jge EndBlockX               ; if so, exit block loop

    ; Compute currentX = x + dx, currentY = y + dy
    lea rax, [r13 + r11]        ; currentX  - horizontal position in the block
    cmp rax, rdx                ; if pixel is out of image width, skip
    jge SkipPixel

    lea rbx, [r14 + r10]        ; currentY - vertical position in the block
    cmp rbx, r8                 ; if pixel is out of image height, skip
    jge SkipPixel

    ; Calculate pixel index in a 32-bit RGBA format:
    ; index = (currentY * width + currentX) * 4
    imul rbx, rdx               ; rbx = currentY * width
    add rbx, rax                ; rbx += currentX
    shl rbx, 2                  ; multiply by 4 (shift left by 2, faster than imul)

    ; Read two pixels from the image data and unpack them to 16-bit values   
    movdqu xmm0, [rcx + rbx]    ; insert 4 BRGA values to register; 
    vpmovzxbd ymm0, xmm0        ; Unpack 8-bit to 32-bit, ymm2 = [B0 G0 R0 A0 | B1 G1 R1 A1]
    vpaddd ymm1, ymm1, ymm0     ; Accumulate 32-bit values, ymm1 += ymm2
    
    add rbp, 2                  ; Increment pixel counter by 2 (we are processing 2 pixels at a time)

SkipPixel:
    ; Move to the next column in the block
    add r11d, 2                 ; dx += 2 because we are processing 2 pixels at a time
    jmp BlockLoopX

EndBlockX:
    ; Go to the next row in the block
    inc r10d                    ; dy++
    jmp BlockLoopY

EndBlockY:
	; Check if we have processed any pixels in the block
    test rbp, rbp               ; if count == 0, skip block
    jz SkipBlock

    ; Pack pixel count to 16-bit values for division
    movd xmm4, ebp              ; Move 32-bit count value to xmm3
    pshufd xmm4, xmm4, 0        ; Broadcast the 32-bit value to all 4 bytes
    cvtdq2ps xmm4, xmm4         ; Convert 32-bit int to float for division (SIMD does not support integer division)

    ; Calculate the average RGBA value for the block
    vmovdqa ymm5, ymm1          ; Copy ymm1 to ymm4 for adding up two pixels from accumulator
    vextracti128 xmm1, ymm1, 1  ; Extract the upper 128 bits of ymm1 into xmm1 | First pixel
    vextracti128 xmm5, ymm5, 0  ; Extract the lower 128 bits of ymm1 into xmm5 | Second pixel

    paddd xmm1, xmm5            ; Add both pixels together
    cvtdq2ps xmm1, xmm1         ; Change values from int to float for division
    divps xmm1, xmm4            ; Divide accumulated data by pixel count of the block
    cvttps2dq xmm1, xmm1        ; Change values back to int

    ; Pack the 32-bit values in xmm1 to 8-bit values for insertion
    packusdw xmm1, xmm1         ; Pack the 32-bit values in xmm1 to 16-bit values
    packuswb xmm1, xmm1         ; Pack the 16-bit values in xmm1 to 8-bit values

    ; Write the average to all pixels in the block
    mov r10d, 0                 ; dy = 0 - set dy for the beginning of the block

WriteBlockY:
    cmp r10d, r9d               ; if reached the end of the block, exit write loop
    jge EndWriteY

    mov r11d, 0                 ; dx = 0 - set dx for the beginning of the block

WriteBlockX:
    cmp r11d, r9d               ; if reached the end of the block, exit write loop
    jge EndWriteX

    ; Compute currentX = x + dx, currentY = y + dy
    lea rax, [r13 + r11]        ; currentX - horizontal position in the block
    cmp eax, edx                ; if pixel is out of image width, skip
    jge SkipWrite

    lea rbx, [r14 + r10]        ; currentY - vertical position in the block
    cmp ebx, r8d                ; if pixel is out of image height, skip
    jge SkipWrite

    ; Calculate pixel index in 32-bit RGBA format
    imul rbx, rdx               ; rbx = currentY * width
    add rbx, rax                ; rbx += currentX
    shl rbx, 2                  ; multiply by 4 (shift left by 2, faster than imul)

    ; Write the average to 4 pixels in the block at a time
    movdqu [rcx + rbx], xmm1

SkipWrite:
    add r11d, 4                 ; adding 4 to dx because we are processing 4 pixels at a time
    jmp WriteBlockX

EndWriteX:
    inc r10d                    ; increment dy - row offset in the block
    jmp WriteBlockY

EndWriteY:
    ; Continue to the next block in the outer loop
    add r13d, r9d               ; x += pixelSize - set x for the start of the next block
    jmp OuterLoopX              ; repeat the process for the next column

SkipBlock:
    ; Skip the block if no pixels were processed
    add r13d, r9d               ; x += pixelSize - set x for the start of the next block
    jmp OuterLoopX              ; repeat the process for the next column

EndLoopX:
    add r14d, r9d               ; y += pixelSize - set y for the start of the next block
    jmp OuterLoopY              ; repeat the process for the next row

EndLoopY:
    ; Restore registers we were using back to their original values from the stack
    pop rbp
    pop r14
    pop r13
    pop rbx

Exit:
    ; Return to the caller
    ret
    
PixelizeImage ENDP
end