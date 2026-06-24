// Hidden C++ exception states: #wind=7
__int64 __fastcall sub_9A5D00(__int64 a1, const void **a2, __int64 a3, unsigned __int8 a4)
{
  size_t v5; // r15
  const void *v6; // r12
  size_t v7; // r14
  unsigned __int64 v8; // rbx
  void **v9; // rdi
  __int64 v10; // rdi
  void **v11; // r13
  __int64 *v12; // rbx
  unsigned int *v13; // r12
  __int64 *v14; // rax
  unsigned __int64 v15; // r15
  void **v16; // rdi
  __int64 v17; // r14
  _QWORD *v18; // rcx
  void **v19; // rdx
  size_t v20; // r8
  __int64 v21; // rax
  __int64 v22; // rcx
  __int64 v23; // r15
  __int64 v24; // rbx
  __int64 v25; // rdx
  __int64 v26; // r8
  __int64 v27; // rax
  void **v28; // r8
  __int64 v30; // rax
  void *v31; // rcx
  unsigned int v32; // ebx
  int v33; // eax
  void *v34; // rcx
  __int64 v35; // rbx
  __int64 v36; // rax
  __int64 *v37; // [rsp+28h] [rbp-C1h]
  void *v38; // [rsp+28h] [rbp-C1h]
  __int128 v39; // [rsp+38h] [rbp-B1h] BYREF
  __int64 v40; // [rsp+48h] [rbp-A1h]
  unsigned __int64 Buf2; // [rsp+50h] [rbp-99h]
  void *Buf2_8[2]; // [rsp+58h] [rbp-91h] BYREF
  size_t v43; // [rsp+68h] [rbp-81h]
  unsigned __int64 v44; // [rsp+70h] [rbp-79h]
  _BYTE *v45; // [rsp+78h] [rbp-71h]
  __int128 v46; // [rsp+80h] [rbp-69h] BYREF
  const char *v47; // [rsp+90h] [rbp-59h]
  __int64 v48; // [rsp+98h] [rbp-51h]
  _BYTE v49[32]; // [rsp+A0h] [rbp-49h] BYREF
  _BYTE v50[32]; // [rsp+C0h] [rbp-29h] BYREF

  v5 = (size_t)a2[2];
  if ( 0x7FFFFFFFFFFFFFFFLL - v5 < 5 )
    unknown_libname_4(0x7FFFFFFFFFFFFFFFLL);
  v6 = a2;
  if ( (unsigned __int64)a2[3] > 0xF )
    v6 = *a2;
  *(_OWORD *)Buf2_8 = 0LL;
  v7 = v5 + 5;
  v8 = 15LL;
  v9 = Buf2_8;
  if ( v5 + 5 > 0xF )
  {
    v8 = v7 | 0xF;
    if ( (v7 | 0xF) <= 0x7FFFFFFFFFFFFFFFLL )
    {
      if ( v8 < 0x16 )
        v8 = 22LL;
    }
    else
    {
      v8 = 0x7FFFFFFFFFFFFFFFLL;
    }
    v9 = (void **)sub_52590(v8 + 1, (__int64)a2, a3);
    Buf2_8[0] = v9;
  }
  v43 = v5 + 5;
  v44 = v8;
  qmemcpy(v9, "swfs/", 5);
  memcpy((char *)v9 + 5, v6, v5);
  *((_BYTE *)v9 + v7) = 0;
  v10 = a1;
  v11 = (void **)(a1 + 280);
  v12 = *(__int64 **)(a1 + 280);
  v13 = (unsigned int *)(a1 + 276);
  v14 = &v12[*(unsigned int *)(a1 + 276)];
  v37 = v14;
  if ( v12 == v14 )
    goto LABEL_22;
  v15 = v44;
  v16 = (void **)Buf2_8[0];
  while ( 1 )
  { // This is the loop that loads swf files, it seems it also checks cross-dependencies meaning
    // it loads from each other based on some unknown sort of references
    v17 = *v12;
    v18 = (_QWORD *)(*v12 + 8);
    v19 = Buf2_8;
    if ( v15 > 0xF )
      v19 = v16;
    v20 = *(_QWORD *)(*v12 + 24);
    if ( *(_QWORD *)(*v12 + 32) > 0xFuLL )
      v18 = (_QWORD *)*v18;
    if ( v20 != v43 )
      goto LABEL_20;
    if ( !v20 || !memcmp(v18, v19, v20) )
      break;
    v14 = v37;
LABEL_20:
    if ( ++v12 == v14 )
    {
      v10 = a1;
LABEL_22:
      v21 = sub_51FB0((__int64)&v39, (__int64)Buf2_8);
      if ( !(unsigned __int8)sub_98F870(v22, v21) )
      {
        *(_QWORD *)&v46 = 0x1300000533LL;
        *((_QWORD *)&v46 + 1) = "C:\\Users\\Tyler\\Desktop\\SVN\\Engine\\code\\engine\\Startup\\Application.cpp";
        v47 = "class glaiel::swf::SWF *__cdecl glaiel::ApplicationBase::load_swf(class std::basic_string<char,struct std:"
              ":char_traits<char>,class std::allocator<char> >,bool,bool,double)";
        v45 = v49;
        v35 = sub_52640(v49, "auto");
        v36 = sub_49090(v50, "SWF File does not exist: ", Buf2_8);
        v39 = v46;
        v40 = (__int64)v47;
        sub_945D70(&unk_12E5AE8, v36, v35, &v39);
      }
      v38 = operator new(0xD0uLL);
      v46 = 0LL;
      v47 = 0LL;
      v48 = 15LL;
      LOBYTE(v46) = 0;
      v23 = sub_9FDB40(v38, &v46);
      v24 = *(_QWORD *)(v10 + 8);
      v39 = 0LL;
      v40 = 0LL;
      Buf2 = 0LL;
      *(_QWORD *)&v39 = sub_52590(0x20uLL, v25, v26);
      v40 = 25LL;
      Buf2 = 31LL;
      strcpy((char *)v39, "per_file_curve_resolution");
      v27 = sub_936440(v24, &v39);
      sub_936440(v27, a2);
      v30 = sub_51FB0((__int64)v49, (__int64)Buf2_8);
      sub_9FDD40(v23, v30, a4);
      if ( Buf2 > 0xF )
      {
        v31 = (void *)v39;
        if ( Buf2 + 1 >= 0x1000 )
        {
          v31 = *(void **)(v39 - 8);
          if ( (unsigned __int64)(v39 - (_QWORD)v31 - 8) > 0x1F )
            invalid_parameter_noinfo_noreturn();
        }
        j_j_free(v31);
      }
      v32 = *v13;
      v33 = *(_DWORD *)(v10 + 272);
      if ( *v13 == v33 )
      {
        v32 = (int)((double)v33 * 1.5);
        if ( v32 < 2 )
          v32 = 2;
        *v11 = j__realloc_base(*v11, 8LL * v32);
        v10 = a1;
        *(_DWORD *)(a1 + 272) = v32;
        if ( v32 >= *v13 )
          v32 = *v13;
        else
          *v13 = v32;
      }
      *((_QWORD *)*v11 + v32) = v23;
      ++*v13;
      sub_A1070(v10 + 696);
      if ( v44 <= 0xF )
        goto LABEL_43;
      v34 = Buf2_8[0];
      if ( v44 + 1 < 0x1000
        || (v34 = (void *)*((_QWORD *)Buf2_8[0] - 1), (unsigned __int64)((char *)Buf2_8[0] - (char *)v34 - 8) <= 0x1F) )
      {
        j_j_free(v34);
LABEL_43:
        sub_522D0((__int64)a2);
        return v23;
      }
LABEL_44:
      invalid_parameter_noinfo_noreturn();
    }
  }
  if ( v15 <= 0xF )
    goto LABEL_28;
  v28 = v16;
  if ( v15 + 1 >= 0x1000 )
  {
    v16 = (void **)*(v16 - 1);
    if ( (unsigned __int64)((char *)v28 - (char *)v16 - 8) > 0x1F )
      goto LABEL_44;
  }
  j_j_free(v16);
LABEL_28:
  sub_522D0((__int64)a2);
  return v17;
}
/*
.text:00000000009A6390 sub_9A6390      proc near               ; CODE XREF: sub_9A4DC0+1C6↑p
.text:00000000009A6390                                         ; DATA XREF: .pdata:000000000145A85C↓o
.text:00000000009A6390
.text:00000000009A6390 Src             = qword ptr -120h
.text:00000000009A6390 var_110         = xmmword ptr -110h
.text:00000000009A6390 var_100         = xmmword ptr -100h
.text:00000000009A6390 var_F0          = qword ptr -0F0h
.text:00000000009A6390 var_E8          = qword ptr -0E8h
.text:00000000009A6390 var_E0          = xmmword ptr -0E0h
.text:00000000009A6390 var_D0          = qword ptr -0D0h
.text:00000000009A6390 var_C8          = qword ptr -0C8h
.text:00000000009A6390 var_C0          = xmmword ptr -0C0h
.text:00000000009A6390 var_B0          = xmmword ptr -0B0h
.text:00000000009A6390 var_A0          = xmmword ptr -0A0h
.text:00000000009A6390 var_90          = xmmword ptr -90h
.text:00000000009A6390 var_80          = xmmword ptr -80h
.text:00000000009A6390 var_70          = qword ptr -70h
.text:00000000009A6390 var_68          = qword ptr -68h
.text:00000000009A6390 var_60          = xmmword ptr -60h
.text:00000000009A6390 var_50          = qword ptr -50h
.text:00000000009A6390 var_48          = byte ptr -48h
.text:00000000009A6390 arg_0           = qword ptr  10h
.text:00000000009A6390 arg_8           = qword ptr  18h
.text:00000000009A6390 arg_10          = qword ptr  20h
.text:00000000009A6390 arg_18          = qword ptr  28h
.text:00000000009A6390
.text:00000000009A6390 ; __unwind { // __CxxFrameHandler4
.text:00000000009A6390                 mov     [rsp-8+arg_18], rbx
.text:00000000009A6395                 mov     [rsp-8+arg_8], rdx
.text:00000000009A639A                 mov     [rsp-8+arg_0], rcx
.text:00000000009A639F                 push    rbp
.text:00000000009A63A0                 push    rsi
.text:00000000009A63A1                 push    rdi
.text:00000000009A63A2                 push    r12
.text:00000000009A63A4                 push    r13
.text:00000000009A63A6                 push    r14
.text:00000000009A63A8                 push    r15
.text:00000000009A63AA                 lea     rbp, [rsp-10h]
.text:00000000009A63AF                 sub     rsp, 110h
.text:00000000009A63B6                 mov     r13, rdx
.text:00000000009A63B9                 xor     r15d, r15d
.text:00000000009A63BC                 mov     rdi, [rdx+78h]
.text:00000000009A63C0                 mov     rbx, [rdi]
.text:00000000009A63C3                 cmp     rbx, rdi
.text:00000000009A63C6                 jz      loc_9A695F
.text:00000000009A63CC                 mov     r10, 7FFFFFFFFFFFFFFFh
.text:00000000009A63D6                 mov     r11d, 16h
.text:00000000009A63DC
.text:00000000009A63DC loc_9A63DC:                             ; CODE XREF: sub_9A6390+5C9↓j
.text:00000000009A63DC                 mov     rax, 5F646E657070415Fh
.text:00000000009A63E6                 lea     r14, [rbx+10h]
.text:00000000009A63EA                 cmp     dword ptr [r14+20h], 0
.text:00000000009A63EF                 jz      loc_9A6953
.text:00000000009A63F5                 xorps   xmm0, xmm0
.text:00000000009A63F8                 movups  [rsp+140h+var_100], xmm0
.text:00000000009A63FD                 mov     [rsp+140h+var_F0], 8
.text:00000000009A6406                 mov     [rsp+140h+var_E8], 0Fh
.text:00000000009A640F                 mov     qword ptr [rsp+140h+var_100], rax
.text:00000000009A6414                 mov     byte ptr [rsp+140h+var_100+8], 0
.text:00000000009A6419                 mov     rsi, [r14+10h]
.text:00000000009A641D                 cmp     rsi, 8
.text:00000000009A6421                 jb      loc_9A6953
.text:00000000009A6427                 mov     r8d, r15d
.text:00000000009A642A                 mov     rcx, r15
.text:00000000009A642D                 mov     r9, [r14+18h]
.text:00000000009A6431
.text:00000000009A6431 loc_9A6431:                             ; CODE XREF: sub_9A6390+C5↓j
.text:00000000009A6431                 mov     rdx, r14
.text:00000000009A6434                 cmp     r9, 0Fh
.text:00000000009A6438                 jbe     short loc_9A643D
.text:00000000009A643A                 mov     rdx, [r14]
.text:00000000009A643D
.text:00000000009A643D loc_9A643D:                             ; CODE XREF: sub_9A6390+A8↑j
.text:00000000009A643D                 movzx   eax, byte ptr [rsp+rcx+140h+var_100]
.text:00000000009A6442                 cmp     [rcx+rdx], al
.text:00000000009A6445                 jnz     loc_9A6953
.text:00000000009A644B                 inc     r8d
.text:00000000009A644E                 inc     rcx
.text:00000000009A6451                 cmp     r8d, 8
.text:00000000009A6455                 jb      short loc_9A6431
.text:00000000009A6457                 xorps   xmm0, xmm0
.text:00000000009A645A                 movups  xmmword ptr [rsp+140h+Src], xmm0
.text:00000000009A645F                 mov     qword ptr [rsp+140h+var_110], r15
.text:00000000009A6464                 mov     qword ptr [rsp+140h+var_110+8], r15
.text:00000000009A6469                 mov     r12, r14
.text:00000000009A646C                 cmp     r9, 0Fh
.text:00000000009A6470                 jbe     short loc_9A6475
.text:00000000009A6472                 mov     r12, [r14]
.text:00000000009A6475
.text:00000000009A6475 loc_9A6475:                             ; CODE XREF: sub_9A6390+E0↑j
.text:00000000009A6475                 cmp     rsi, r10
.text:00000000009A6478                 ja      loc_9A6980
.text:00000000009A647E                 cmp     rsi, 0Fh
.text:00000000009A6482                 ja      short loc_9A64A0
.text:00000000009A6484                 mov     qword ptr [rsp+140h+var_110], rsi
.text:00000000009A6489                 mov     r10d, 0Fh
.text:00000000009A648F                 mov     qword ptr [rsp+140h+var_110+8], r10
.text:00000000009A6494                 movups  xmm0, xmmword ptr [r12]
.text:00000000009A6499                 movups  xmmword ptr [rsp+140h+Src], xmm0
.text:00000000009A649E                 jmp     short loc_9A64ED
.text:00000000009A64A0 ; ---------------------------------------------------------------------------
.text:00000000009A64A0
.text:00000000009A64A0 loc_9A64A0:                             ; CODE XREF: sub_9A6390+F2↑j
.text:00000000009A64A0                 mov     r15, rsi
.text:00000000009A64A3                 or      r15, 0Fh
.text:00000000009A64A7                 cmp     r15, r10
.text:00000000009A64AA                 jbe     short loc_9A64B1
.text:00000000009A64AC                 mov     r15, r10
.text:00000000009A64AF                 jmp     short loc_9A64B9
.text:00000000009A64B1 ; ---------------------------------------------------------------------------
.text:00000000009A64B1
.text:00000000009A64B1 loc_9A64B1:                             ; CODE XREF: sub_9A6390+11A↑j
.text:00000000009A64B1                 cmp     r15, 16h
.text:00000000009A64B5                 cmovb   r15, r11
.text:00000000009A64B9
.text:00000000009A64B9 loc_9A64B9:                             ; CODE XREF: sub_9A6390+11F↑j
.text:00000000009A64B9                 lea     rcx, [r15+1]
.text:00000000009A64BD                 call    sub_52590
.text:00000000009A64C2                 mov     [rsp+140h+Src], rax
.text:00000000009A64C7                 mov     qword ptr [rsp+140h+var_110], rsi
.text:00000000009A64CC                 mov     qword ptr [rsp+140h+var_110+8], r15
.text:00000000009A64D1                 lea     r8, [rsi+1]     ; Size
.text:00000000009A64D5                 mov     rdx, r12        ; Src
.text:00000000009A64D8                 mov     rcx, rax        ; void *
.text:00000000009A64DB                 call    memcpy
.text:00000000009A64E0                 mov     r10, qword ptr [rsp+140h+var_110+8]
.text:00000000009A64E5                 mov     rsi, qword ptr [rsp+140h+var_110]
.text:00000000009A64EA                 xor     r15d, r15d
.text:00000000009A64ED
.text:00000000009A64ED loc_9A64ED:                             ; CODE XREF: sub_9A6390+10E↑j
.text:00000000009A64ED                 xorps   xmm0, xmm0
.text:00000000009A64F0                 movups  [rsp+140h+var_100], xmm0
.text:00000000009A64F5                 mov     [rsp+140h+var_F0], 8
.text:00000000009A64FE                 mov     [rsp+140h+var_E8], 0Fh
.text:00000000009A6507                 mov     rax, 5F646E657070415Fh
.text:00000000009A6511                 mov     qword ptr [rsp+140h+var_100], rax
.text:00000000009A6516                 mov     byte ptr [rsp+140h+var_100+8], 0
.text:00000000009A651B                 mov     r8, [rsp+140h+Src]
.text:00000000009A6520                 cmp     rsi, 8
.text:00000000009A6524                 jb      loc_9A6620
.text:00000000009A652A                 mov     r9d, r15d
.text:00000000009A652D                 mov     rax, r15
.text:00000000009A6530
.text:00000000009A6530 loc_9A6530:                             ; CODE XREF: sub_9A6390+1C5↓j
.text:00000000009A6530                 lea     rdx, [rsp+140h+Src]
.text:00000000009A6535                 cmp     r10, 0Fh
.text:00000000009A6539                 cmova   rdx, r8
.text:00000000009A653D                 movzx   ecx, byte ptr [rsp+rax+140h+var_100]
.text:00000000009A6542                 cmp     [rax+rdx], cl
.text:00000000009A6545                 jnz     loc_9A6620
.text:00000000009A654B                 inc     r9d
.text:00000000009A654E                 inc     rax
.text:00000000009A6551                 cmp     r9d, 8
.text:00000000009A6555                 jb      short loc_9A6530
.text:00000000009A6557                 lea     rcx, [rsp+140h+Src]
.text:00000000009A655C                 cmp     r10, 0Fh
.text:00000000009A6560                 cmova   rcx, r8
.text:00000000009A6564                 add     rcx, rsi
.text:00000000009A6567                 lea     rdx, [rsp+140h+Src]
.text:00000000009A656C                 cmp     r10, 0Fh
.text:00000000009A6570                 cmova   rdx, r8
.text:00000000009A6574                 add     rdx, 8
.text:00000000009A6578                 xorps   xmm0, xmm0
.text:00000000009A657B                 movups  [rbp+40h+var_A0], xmm0
.text:00000000009A657F                 mov     qword ptr [rbp+40h+var_90], r15
.text:00000000009A6583                 mov     qword ptr [rbp+40h+var_90+8], r15
.text:00000000009A6587                 cmp     rdx, rcx
.text:00000000009A658A                 jnz     short loc_9A659A
.text:00000000009A658C                 mov     qword ptr [rbp+40h+var_90+8], 0Fh
.text:00000000009A6594                 mov     byte ptr [rbp+40h+var_A0], 0
.text:00000000009A6598                 jmp     short loc_9A65B3
.text:00000000009A659A ; ---------------------------------------------------------------------------
.text:00000000009A659A
.text:00000000009A659A loc_9A659A:                             ; CODE XREF: sub_9A6390+1FA↑j
.text:00000000009A659A                 sub     rcx, rdx
.text:00000000009A659D                 mov     r8, rcx
.text:00000000009A65A0                 lea     rcx, [rbp+40h+var_A0]
.text:00000000009A65A4                 call    sub_50EA0
.text:00000000009A65A9                 mov     r10, qword ptr [rsp+140h+var_110+8]
.text:00000000009A65AE                 mov     r8, [rsp+140h+Src]
.text:00000000009A65B3
.text:00000000009A65B3 loc_9A65B3:                             ; CODE XREF: sub_9A6390+208↑j
.text:00000000009A65B3                 cmp     r10, 0Fh
.text:00000000009A65B7                 jbe     short loc_9A65EA
.text:00000000009A65B9                 lea     rdx, [r10+1]
.text:00000000009A65BD                 mov     rax, r8
.text:00000000009A65C0                 cmp     rdx, 1000h
.text:00000000009A65C7                 jb      short loc_9A65E2
.text:00000000009A65C9                 add     rdx, 27h ; '''
.text:00000000009A65CD                 mov     r8, [r8-8]
.text:00000000009A65D1                 sub     rax, r8
.text:00000000009A65D4                 add     rax, 0FFFFFFFFFFFFFFF8h
.text:00000000009A65D8                 cmp     rax, 1Fh
.text:00000000009A65DC                 ja      loc_9A6986
.text:00000000009A65E2
.text:00000000009A65E2 loc_9A65E2:                             ; CODE XREF: sub_9A6390+237↑j
.text:00000000009A65E2                 mov     rcx, r8         ; Block
.text:00000000009A65E5                 call    j_j_free
.text:00000000009A65EA
.text:00000000009A65EA loc_9A65EA:                             ; CODE XREF: sub_9A6390+227↑j
.text:00000000009A65EA                 movups  xmm2, [rbp+40h+var_A0]
.text:00000000009A65EE                 movups  xmmword ptr [rsp+140h+Src], xmm2
.text:00000000009A65F3                 movups  xmm1, [rbp+40h+var_90]
.text:00000000009A65F7                 movups  [rsp+140h+var_110], xmm1
.text:00000000009A65FC                 mov     qword ptr [rbp+40h+var_90+8], 0Fh
.text:00000000009A6604                 mov     byte ptr [rbp+40h+var_A0], 0
.text:00000000009A6608                 movdqa  xmm0, xmm1
.text:00000000009A660C                 psrldq  xmm0, 8
.text:00000000009A6611                 movq    r10, xmm0
.text:00000000009A6616                 movq    rsi, xmm1
.text:00000000009A661B                 movq    r8, xmm2
.text:00000000009A6620
.text:00000000009A6620 loc_9A6620:                             ; CODE XREF: sub_9A6390+194↑j
.text:00000000009A6620                                         ; sub_9A6390+1B5↑j
.text:00000000009A6620                 xorps   xmm0, xmm0
.text:00000000009A6623                 movups  [rsp+140h+var_E0], xmm0
.text:00000000009A6628                 mov     [rsp+140h+var_D0], r15
.text:00000000009A662D                 mov     [rsp+140h+var_C8], r15
.text:00000000009A6632                 lea     r12, [rsp+140h+Src]
.text:00000000009A6637                 cmp     r10, 0Fh
.text:00000000009A663B                 cmova   r12, r8
.text:00000000009A663F                 mov     rax, 7FFFFFFFFFFFFFFFh
.text:00000000009A6649                 cmp     rsi, rax
.text:00000000009A664C                 ja      loc_9A697A
.text:00000000009A6652                 cmp     rsi, 0Fh
.text:00000000009A6656                 ja      short loc_9A6672
.text:00000000009A6658                 mov     [rsp+140h+var_D0], rsi
.text:00000000009A665D                 mov     [rsp+140h+var_C8], 0Fh
.text:00000000009A6666                 movups  xmm0, xmmword ptr [r12]
.text:00000000009A666B                 movups  [rsp+140h+var_E0], xmm0
.text:00000000009A6670                 jmp     short loc_9A66B7
.text:00000000009A6672 ; ---------------------------------------------------------------------------
.text:00000000009A6672
.text:00000000009A6672 loc_9A6672:                             ; CODE XREF: sub_9A6390+2C6↑j
.text:00000000009A6672                 mov     r15, rsi
.text:00000000009A6675                 or      r15, 0Fh
.text:00000000009A6679                 cmp     r15, rax
.text:00000000009A667C                 jbe     short loc_9A6683
.text:00000000009A667E                 mov     r15, rax
.text:00000000009A6681                 jmp     short loc_9A6690
.text:00000000009A6683 ; ---------------------------------------------------------------------------
.text:00000000009A6683
.text:00000000009A6683 loc_9A6683:                             ; CODE XREF: sub_9A6390+2EC↑j
.text:00000000009A6683                 cmp     r15, 16h
.text:00000000009A6687                 mov     eax, 16h
.text:00000000009A668C                 cmovb   r15, rax
.text:00000000009A6690
.text:00000000009A6690 loc_9A6690:                             ; CODE XREF: sub_9A6390+2F1↑j
.text:00000000009A6690                 lea     rcx, [r15+1]
.text:00000000009A6694                 call    sub_52590
.text:00000000009A6699                 mov     qword ptr [rsp+140h+var_E0], rax
.text:00000000009A669E                 mov     [rsp+140h+var_D0], rsi
.text:00000000009A66A3                 mov     [rsp+140h+var_C8], r15
.text:00000000009A66A8                 lea     r8, [rsi+1]     ; Size
.text:00000000009A66AC                 mov     rdx, r12        ; Src
.text:00000000009A66AF                 mov     rcx, rax        ; void *
.text:00000000009A66B2                 call    memcpy
.text:00000000009A66B7
.text:00000000009A66B7 loc_9A66B7:                             ; CODE XREF: sub_9A6390+2E0↑j
.text:00000000009A66B7                 lea     rdx, [rsp+140h+var_E0]
.text:00000000009A66BC                 mov     rcx, [rbp+40h+arg_0]
.text:00000000009A66C0                 call    sub_9A5A10
.text:00000000009A66C5                 mov     rsi, rax
.text:00000000009A66C8                 lea     rcx, [r13+70h]
.text:00000000009A66CC                 mov     r8, r14
.text:00000000009A66CF                 lea     rdx, [rbp+40h+var_48]
.text:00000000009A66D3                 call    sub_121CA0
.text:00000000009A66D8                 mov     rcx, [rax]
.text:00000000009A66DB                 mov     edx, [rcx+30h]
.text:00000000009A66DE                 test    edx, edx
.text:00000000009A66E0                 jnz     short loc_9A66EA
.text:00000000009A66E2                 xor     r15d, r15d
.text:00000000009A66E5                 mov     eax, r15d
.text:00000000009A66E8                 jmp     short loc_9A66F6
.text:00000000009A66EA ; ---------------------------------------------------------------------------
.text:00000000009A66EA
.text:00000000009A66EA loc_9A66EA:                             ; CODE XREF: sub_9A6390+350↑j
.text:00000000009A66EA                 lea     rcx, [r13+30h]
.text:00000000009A66EE                 call    sub_A364C0
.text:00000000009A66F3                 xor     r15d, r15d
.text:00000000009A66F6
.text:00000000009A66F6 loc_9A66F6:                             ; CODE XREF: sub_9A6390+358↑j
.text:00000000009A66F6                 test    rsi, rsi
.text:00000000009A66F9                 jz      short loc_9A6710
.text:00000000009A66FB                 test    rax, rax
.text:00000000009A66FE                 jz      short loc_9A6710
.text:00000000009A6700                 mov     rdx, rax
.text:00000000009A6703                 mov     rcx, rsi
.text:00000000009A6706                 call    sub_A462F0
.text:00000000009A670B                 jmp     loc_9A690A
.text:00000000009A6710 ; ---------------------------------------------------------------------------
.text:00000000009A6710
.text:00000000009A6710 loc_9A6710:                             ; CODE XREF: sub_9A6390+369↑j
.text:00000000009A6710                                         ; sub_9A6390+36E↑j
.text:00000000009A6710                 mov     dword ptr [rbp+40h+var_60], 555h
.text:00000000009A6717                 mov     dword ptr [rbp+40h+var_60+4], 1Fh
.text:00000000009A671E                 lea     rax, aCUsersTylerDes_61 ; "C:\\Users\\Tyler\\Desktop\\SVN\\Engine"...
.text:00000000009A6725                 mov     qword ptr [rbp+40h+var_60+8], rax
.text:00000000009A6729                 lea     rax, aVoidCdeclGlaie_77 ; "void __cdecl glaiel::ApplicationBase::p"...
.text:00000000009A6730                 mov     [rbp+40h+var_50], rax
.text:00000000009A6734                 lea     rax, [rsp+140h+var_100]
.text:00000000009A6739                 mov     [rbp+40h+arg_10], rax
.text:00000000009A673D                 xorps   xmm0, xmm0
.text:00000000009A6740                 movups  [rsp+140h+var_100], xmm0
.text:00000000009A6745                 mov     [rsp+140h+var_F0], 4
.text:00000000009A674E                 mov     [rsp+140h+var_E8], 0Fh
.text:00000000009A6757                 mov     dword ptr [rsp+140h+var_100], 6F747561h
.text:00000000009A675F                 mov     byte ptr [rsp+140h+var_100+4], 0
.text:00000000009A6764                 mov     r13, [rbx+20h]
.text:00000000009A6768                 mov     rcx, 7FFFFFFFFFFFFFFFh
.text:00000000009A6772                 mov     rax, rcx
.text:00000000009A6775                 sub     rax, r13
.text:00000000009A6778                 cmp     rax, 12h
.text:00000000009A677C                 jb      loc_9A699E
.text:00000000009A6782                 cmp     qword ptr [rbx+28h], 0Fh
.text:00000000009A6787                 jbe     short loc_9A678C
.text:00000000009A6789                 mov     r14, [r14]
.text:00000000009A678C
.text:00000000009A678C loc_9A678C:                             ; CODE XREF: sub_9A6390+3F7↑j
.text:00000000009A678C                 movups  [rbp+40h+var_80], xmm0
.text:00000000009A6790                 mov     [rbp+40h+var_70], r15
.text:00000000009A6794                 mov     [rbp+40h+var_68], r15
.text:00000000009A6798                 lea     r12, [r13+12h]
.text:00000000009A679C                 mov     esi, 0Fh
.text:00000000009A67A1                 lea     r15, [rbp+40h+var_80]
.text:00000000009A67A5                 cmp     r12, rsi
.text:00000000009A67A8                 jbe     short loc_9A67D8
.text:00000000009A67AA                 mov     rsi, r12
.text:00000000009A67AD                 or      rsi, 0Fh
.text:00000000009A67B1                 cmp     rsi, rcx
.text:00000000009A67B4                 jbe     short loc_9A67BB
.text:00000000009A67B6                 mov     rsi, rcx
.text:00000000009A67B9                 jmp     short loc_9A67C8
.text:00000000009A67BB ; ---------------------------------------------------------------------------
.text:00000000009A67BB
.text:00000000009A67BB loc_9A67BB:                             ; CODE XREF: sub_9A6390+424↑j
.text:00000000009A67BB                 cmp     rsi, 16h
.text:00000000009A67BF                 mov     eax, 16h
.text:00000000009A67C4                 cmovb   rsi, rax
.text:00000000009A67C8
.text:00000000009A67C8 loc_9A67C8:                             ; CODE XREF: sub_9A6390+429↑j
.text:00000000009A67C8                 lea     rcx, [rsi+1]
.text:00000000009A67CC                 call    sub_52590
.text:00000000009A67D1                 mov     r15, rax
.text:00000000009A67D4                 mov     qword ptr [rbp+40h+var_80], rax
.text:00000000009A67D8
.text:00000000009A67D8 loc_9A67D8:                             ; CODE XREF: sub_9A6390+418↑j
.text:00000000009A67D8                 mov     [rbp+40h+var_70], r12
.text:00000000009A67DC                 mov     [rbp+40h+var_68], rsi
.text:00000000009A67E0                 movups  xmm0, cs:xmmword_111E538
.text:00000000009A67E7                 movups  xmmword ptr [r15], xmm0
.text:00000000009A67EB                 movzx   ecx, cs:word_111E548
.text:00000000009A67F2                 mov     [r15+10h], cx
.text:00000000009A67F7                 lea     rcx, [r15+12h]  ; void *
.text:00000000009A67FB                 mov     r8, r13         ; Size
.text:00000000009A67FE                 mov     rdx, r14        ; Src
.text:00000000009A6801                 call    memcpy
.text:00000000009A6806                 mov     byte ptr [r15+r12], 0
.text:00000000009A680B                 mov     r8d, 31h ; '1'
.text:00000000009A6811                 lea     rdx, aCouldNotLocate_0 ; " could not locate original movieclip to"...
.text:00000000009A6818                 lea     rcx, [rbp+40h+var_80]
.text:00000000009A681C                 call    concat_string
.text:00000000009A6821                 xorps   xmm0, xmm0
.text:00000000009A6824                 movups  [rbp+40h+var_C0], xmm0
.text:00000000009A6828                 xor     r15d, r15d
.text:00000000009A682B                 mov     qword ptr [rbp+40h+var_B0], r15
.text:00000000009A682F                 mov     qword ptr [rbp+40h+var_B0+8], r15
.text:00000000009A6833                 movups  xmm0, xmmword ptr [rax]
.text:00000000009A6836                 movups  [rbp+40h+var_C0], xmm0
.text:00000000009A683A                 movups  xmm1, xmmword ptr [rax+10h]
.text:00000000009A683E                 movups  [rbp+40h+var_B0], xmm1
.text:00000000009A6842                 mov     [rax+10h], r15
.text:00000000009A6846                 mov     qword ptr [rax+18h], 0Fh
.text:00000000009A684E                 mov     [rax], r15b
.text:00000000009A6851                 movups  xmm0, [rbp+40h+var_60]
.text:00000000009A6855                 movaps  [rsp+140h+var_E0], xmm0
.text:00000000009A685A                 movsd   xmm1, [rbp+40h+var_50]
.text:00000000009A685F                 movsd   [rsp+140h+var_D0], xmm1
.text:00000000009A6865                 lea     r9, [rsp+140h+var_E0]
.text:00000000009A686A                 lea     r8, [rsp+140h+var_100]
.text:00000000009A686F                 lea     rdx, [rbp+40h+var_C0]
.text:00000000009A6873                 lea     rcx, unk_12E5AE8
.text:00000000009A687A                 call    sub_946150
.text:00000000009A687F                 nop
.text:00000000009A6880                 mov     rdx, qword ptr [rbp+40h+var_B0+8]
.text:00000000009A6884                 cmp     rdx, 0Fh
.text:00000000009A6888                 jbe     short loc_9A68BB
.text:00000000009A688A                 inc     rdx
.text:00000000009A688D                 mov     rcx, qword ptr [rbp+40h+var_C0]
.text:00000000009A6891                 mov     rax, rcx
.text:00000000009A6894                 cmp     rdx, 1000h
.text:00000000009A689B                 jb      short loc_9A68B6
.text:00000000009A689D                 add     rdx, 27h ; '''
.text:00000000009A68A1                 mov     rcx, [rcx-8]    ; Block
.text:00000000009A68A5                 sub     rax, rcx
.text:00000000009A68A8                 add     rax, 0FFFFFFFFFFFFFFF8h
.text:00000000009A68AC                 cmp     rax, 1Fh
.text:00000000009A68B0                 ja      loc_9A698C
.text:00000000009A68B6
.text:00000000009A68B6 loc_9A68B6:                             ; CODE XREF: sub_9A6390+50B↑j
.text:00000000009A68B6                 call    j_j_free
.text:00000000009A68BB
.text:00000000009A68BB loc_9A68BB:                             ; CODE XREF: sub_9A6390+4F8↑j
.text:00000000009A68BB                 mov     qword ptr [rbp+40h+var_B0], r15
.text:00000000009A68BF                 mov     qword ptr [rbp+40h+var_B0+8], 0Fh
.text:00000000009A68C7                 mov     byte ptr [rbp+40h+var_C0], 0
.text:00000000009A68CB                 mov     rdx, [rbp+40h+var_68]
.text:00000000009A68CF                 cmp     rdx, 0Fh
.text:00000000009A68D3                 jbe     short loc_9A6906
.text:00000000009A68D5                 inc     rdx
.text:00000000009A68D8                 mov     rcx, qword ptr [rbp+40h+var_80]
.text:00000000009A68DC                 mov     rax, rcx
.text:00000000009A68DF                 cmp     rdx, 1000h
.text:00000000009A68E6                 jb      short loc_9A6901
.text:00000000009A68E8                 add     rdx, 27h ; '''
.text:00000000009A68EC                 mov     rcx, [rcx-8]    ; Block
.text:00000000009A68F0                 sub     rax, rcx
.text:00000000009A68F3                 add     rax, 0FFFFFFFFFFFFFFF8h
.text:00000000009A68F7                 cmp     rax, 1Fh
.text:00000000009A68FB                 ja      loc_9A6992
.text:00000000009A6901
.text:00000000009A6901 loc_9A6901:                             ; CODE XREF: sub_9A6390+556↑j
.text:00000000009A6901                 call    j_j_free
.text:00000000009A6906
.text:00000000009A6906 loc_9A6906:                             ; CODE XREF: sub_9A6390+543↑j
.text:00000000009A6906                 mov     r13, [rbp+40h+arg_8]
.text:00000000009A690A
.text:00000000009A690A loc_9A690A:                             ; CODE XREF: sub_9A6390+37B↑j
.text:00000000009A690A                 mov     rdx, qword ptr [rsp+140h+var_110+8]
.text:00000000009A690F                 cmp     rdx, 0Fh
.text:00000000009A6913                 jbe     short loc_9A6943
.text:00000000009A6915                 inc     rdx
.text:00000000009A6918                 mov     rcx, [rsp+140h+Src]
.text:00000000009A691D                 mov     rax, rcx
.text:00000000009A6920                 cmp     rdx, 1000h
.text:00000000009A6927                 jb      short loc_9A693E
.text:00000000009A6929                 add     rdx, 27h ; '''
.text:00000000009A692D                 mov     rcx, [rcx-8]    ; Block
.text:00000000009A6931                 sub     rax, rcx
.text:00000000009A6934                 add     rax, 0FFFFFFFFFFFFFFF8h
.text:00000000009A6938                 cmp     rax, 1Fh
.text:00000000009A693C                 ja      short loc_9A6998
.text:00000000009A693E
.text:00000000009A693E loc_9A693E:                             ; CODE XREF: sub_9A6390+597↑j
.text:00000000009A693E                 call    j_j_free
.text:00000000009A6943
.text:00000000009A6943 loc_9A6943:                             ; CODE XREF: sub_9A6390+583↑j
.text:00000000009A6943                 mov     r11d, 16h
.text:00000000009A6949                 mov     r10, 7FFFFFFFFFFFFFFFh
.text:00000000009A6953
.text:00000000009A6953 loc_9A6953:                             ; CODE XREF: sub_9A6390+5F↑j
.text:00000000009A6953                                         ; sub_9A6390+91↑j ...
.text:00000000009A6953                 mov     rbx, [rbx]
.text:00000000009A6956                 cmp     rbx, rdi
.text:00000000009A6959                 jnz     loc_9A63DC
.text:00000000009A695F
.text:00000000009A695F loc_9A695F:                             ; CODE XREF: sub_9A6390+36↑j
.text:00000000009A695F                 mov     rbx, [rsp+140h+arg_18]
.text:00000000009A6967                 add     rsp, 110h
.text:00000000009A696E                 pop     r15
.text:00000000009A6970                 pop     r14
.text:00000000009A6972                 pop     r13
.text:00000000009A6974                 pop     r12
.text:00000000009A6976                 pop     rdi
.text:00000000009A6977                 pop     rsi
.text:00000000009A6978                 pop     rbp
.text:00000000009A6979                 retn
*/