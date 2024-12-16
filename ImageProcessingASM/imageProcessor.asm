.code
processImage PROC

    movdqu xmm0, [rcx]
    movdqu xmm1, [rdx]
    psubw xmm0, xmm1
    movdqu [r8], xmm0

    ret

processImage ENDP
end