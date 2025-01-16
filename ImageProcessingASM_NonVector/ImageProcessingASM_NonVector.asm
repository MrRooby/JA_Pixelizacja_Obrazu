;-------------------------------------------------------------------------------------------;
;-------------------------------------------------------------------------------------------;
;  _____ _______   ________ _      _____ ____________   _____ __  __          _____ ______  ;
; |  __ \_   _\ \ / /  ____| |    |_   _|___  /  ____| |_   _|  \/  |   /\   / ____|  ____| ;
; | |__) || |  \ V /| |__  | |      | |    / /| |__      | | | \  / |  /  \ | |  __| |__    ;
; |  ___/ | |   > < |  __| | |      | |   / / |  __|     | | | |\/| | / /\ \| | |_ |  __|   ;
; | |    _| |_ / . \| |____| |____ _| |_ / /__| |____   _| |_| |  | |/ ____ \ |__| | |____  ;
; |_|   |_____/_/ \_\______|______|_____/_____|______| |_____|_|  |_/_/    \_\_____|______| ;
;-------------------------------------------------------------------------------------------;
;                               STANDARD REGISTER VERSION                                   ;
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
;  R8  -> height    ; INTEGER Height of the image in pixels                                 ;
;  R9  -> pixelSize ; INTEGER Size of the pixel block for averaging                         ;
;                                                                                           ;
; Error Handling:                                                                           ;
;   - If pixelSize < 1, the procedure exits immediately.                                    ;
;   - Ensure that width and height are positive integers.                                   ;
;   - Ensure that imageData is a valid pointer to the image data.                           ;
;                                                                                           ;
; ASSUMPTIONS (program does not check if true):                                             ;
;   - The image data is stored in 32-bit RGBA format.                                       ;
;                                                                                           ;
; Technologies Used:                                                                        ;
;   - Assembly language (x86-64)                                                            ;
;                                                                                           ;
;-------------------------------------------------------------------------------------------;
;-------------------------------------------------------------------------------------------;

.code
PixelizeImage PROC
    ; Check if pixelSize < 1, if so exit immediately
    cmp r9d, 1                              ; Compare pixelSize to 1
    jl Exit                                 ; Jump to Exit if pixelSize < 1

    ; Save registers that will be used throughout the procedure
    ; so we can restore them before exiting
    push rbx
    push rsi
    push rdi
    push r12
    push r13
    push r14
    push r15
    push rbp

    ; Initialize outer loop variable (block row) y -> r14d = 0
    xor r14d, r14d                          ; r14d holds y, set to 0

OuterLoopY: ; Loop iterates over rows (y) by adding pixelSize every iteration of the image until height is reached
    ; Compare current y with height (r8d)
    cmp r14d, r8d                           ; have we reached the end of the image?
    jge EndLoopY                            ; if so, exit outer loop

    ; Initialize x for inner loop -> r13d = 0
    xor r13d, r13d                          ; r13d holds x, set to 0

OuterLoopX: ; Loop iterates over columns (x) adding pixelSize every iteration of the image until width is reached
    ; Compare current x with width (edx)
    cmp r13d, edx                           ; have we reached the end of the row?
    jge EndLoopX                            ; if so, exit inner loop

    ; 5) Initialize accumulators for this block and set them to 0:
    xor r12, r12                            ; Accumulator for B channel (sumB) 
    xor r15, r15                            ; Accumulator for G channel (sumG)
    xor rsi, rsi                            ; Accumulator for R channel (sumR)
    xor rdi, rdi                            ; Accumulator for A channel (sumA)

    ; Initialize the counter for the number of pixels accumulated in the block
    xor rbp, rbp                            ; rbp holds count, set to 0 

    ; Loop variables for block iteration:
    ; dy = r10d - row offset in the block
    ; dx = r11d - column offset in the block
    mov r10d, 0                             ; dy = 0

BlockLoopY:
    ; Check if we have reached the end of the block
    cmp r10d, r9d                           ; if dy >= pixelSize
    jge EndBlockY                           ; if so, exit block loop

    ; Start with dx = 0
    mov r11d, 0                             ; dx = 0

BlockLoopX:
    ; Check if we have reached the end of the block
    cmp r11d, r9d                           ; if dx >= pixelSize
    jge EndBlockX                           ; if so, exit block loop

    ; Compute currentX = x + dx, currentY = y + dy
    lea rax, [r13 + r11]                    ; currentX  - horizontal position in the block
    cmp rax, rdx                            ; if pixel is out of image width, skip
    jge SkipPixel

    lea rbx, [r14 + r10]                    ; currentY - vertical position in the block
    cmp rbx, r8                             ; if pixel is out of image height, skip
    jge SkipPixel

    ; Calculate pixel index in a 32-bit RGBA format:
    ; index = (currentY * width + currentX) * 4
    imul rbx, rdx                           ; rbx = currentY * width
    add rbx, rax                            ; rbx += currentX
    shl rbx, 2                              ; multiply by 4 (shift left by 2, faster than imul)

    ; Read the 4 channels (B, G, R, A) from the image memory and accumulate
    movzx rax, byte ptr [rcx + rbx]         ; read B channel
    add r12, rax                            ; accumulate B channel

    movzx rax, byte ptr [rcx + rbx + 1]     ; read G channel
    add r15, rax                            ; accumulate G channel

    movzx rax, byte ptr [rcx + rbx + 2]     ; read R channel
    add rsi, rax                            ; accumulate R channel                              

    movzx rax, byte ptr [rcx + rbx + 3]     ; read A channel
    add rdi, rax                            ; accumulate A channel

    inc rbp                                 ; count++

SkipPixel:
    ; Move to the next column in the block
    inc r11d                                ; dx++
    jmp BlockLoopX

EndBlockX:
    ; Go to the next row in the block
    inc r10d                                ; dy++
    jmp BlockLoopY

EndBlockY:
	; Check if we have processed any pixels in the block
    test rbp, rbp                           ; if count == 0, skip block
    jz SkipBlock

    ; saving rdx register because cdq instruction overrides it
    ; and we will need its value later
    push rdx

    ; Calculate the average color values for the block
    ; Blue channel
    xor rax, rax                            ; clear rax register
    mov eax, r12d                           ; put sumB into eax
    cdq                                     ; sign extend for division 
                                            ; eax -> stores quotient, edx -> stores remainder 
    idiv rbp                                ; divide value by count of pixels in the block
    movzx r12d, al                          ; put averaged value back into r12d

    ; Green channel
    xor rax, rax                            ; clear rax register
    mov eax, r15d                           ; put sumG into eax
    cdq                                     ; sign extend for division 
                                            ; eax -> stores quotient, edx -> stores remainder 
    idiv rbp                                ; divide value by count of pixels in the block
    movzx r15d, al                          ; put averaged value back into r15d

    ; Red channel
    xor rax, rax                            ; clear rax register
    mov eax, esi                            ; put sumR into eax
    cdq                                     ; sign extend for division 
                                            ; eax -> stores quotient, edx -> stores remainder 
    idiv rbp                                ; divide value by count of pixels in the block
    movzx rsi, al                           ; put averaged value back into rsi

    ; Alpha channel
    xor rax, rax                            ; clear rax register
    mov eax, edi                            ; put sumA into eax
    cdq                                     ; sign extend for division 
                                            ; eax -> stores quotient, edx -> stores remainder 
    idiv rbp                                ; divide value by count of pixels in the block
    movzx rdi, al                           ; put averaged value back into rdi

    pop rdx                                 ; restoring rdx register after average calculation

    ; Write the average to all pixels in the block
    mov r10d, 0                             ; dy = 0 - set dy for the beginning of the block

WriteBlockY:
    cmp r10d, r9d                           ; if reached the end of the block, exit write loop
    jge EndWriteY

    mov r11d, 0                             ; dx = 0 - set dx for the beginning of the block

WriteBlockX:
    cmp r11d, r9d                           ; if reached the end of the block, exit write loop
    jge EndWriteX

    ; Compute currentX = x + dx, currentY = y + dy
    lea rax, [r13 + r11]                    ; currentX - horizontal position in the block
    cmp eax, edx                            ; if pixel is out of image width, skip
    jge SkipWrite

    lea rbx, [r14 + r10]                    ; currentY - vertical position in the block
    cmp ebx, r8d                            ; if pixel is out of image height, skip
    jge SkipWrite

    ; Calculate pixel index in 32-bit RGBA format
    imul rbx, rdx                           ; rbx = currentY * width
    add rbx, rax                            ; rbx += currentX
    shl rbx, 2                              ; multiply by 4 (shift left by 2, faster than imul)

    ; Write average values back to the pixel
    mov byte ptr [rcx + rbx], r12b          ; B
    mov byte ptr [rcx + rbx + 1], r15b      ; G
    mov byte ptr [rcx + rbx + 2], sil       ; R
    mov byte ptr [rcx + rbx + 3], dil       ; A

SkipWrite:
    inc r11d                                ; dx++
    jmp WriteBlockX

EndWriteX:
    inc r10d                                ; dy++
    jmp WriteBlockY

EndWriteY:
    ; Continue to the next block in the outer loop
    add r13d, r9d                           ; x += pixelSize - set x for the start of the next block
    jmp OuterLoopX                          ; repeat the process for the next column

SkipBlock:
    ; Skip the block if no pixels were processed
    add r13d, r9d                           ; x += pixelSize - set x for the start of the next block
    jmp OuterLoopX                          ; repeat the process for the next column

EndLoopX:
    add r14d, r9d                           ; y += pixelSize - set y for the start of the next block
    jmp OuterLoopY                          ; repeat the process for the next row

EndLoopY:
    ; Restore registers we were using back to their original values from the stack
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