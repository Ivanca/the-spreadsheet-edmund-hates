Im trying to mod a game and I found where it loads its graphics, by graphics I mean SWF files that it uses for this purpouse.

Thanks to a mod framework I found online I'm able to add a SWF of my own to the loading pipeline, but that's it, its being loaded but its not showing at all, I need to be able to show it (its called x.swf)

At sub_9A5D00 (attached below) is where the swf files are loaded, I can see that my SWF file is present in the iteration.

I have captured the spawning of another visual element to shred light into how they are being instantiated and shown, the element its called "CatStatsDrawer" and the callstack its attached below, the subroutine is called 8 times: object_cinderblock1, small_trash_can2, object_box, object_cinderblock1, wallmounted_picture_sailboat, object_box, object_cattree1, wallmounted_picture_sailboat. To be clear there is just one wall mounted sailboat picture  (not 2 as the loop might suggest)

I need to know how to activate the SWF file so its visible. I know that the information here may be insufficient to achieve that goal, therefore I would like you to write a IDA python script (and/or frida script) that would give you the information missing that you need to achieve that goal.


########################################################################################################################
Function: sub_1A11E0 
Start EA: 0x1A11E0 (RVA)
########################################################################################################################

Callstack addresses found:
    0x1A1D01

__int64 __fastcall sub_1A11E0(__int64 a1, void **a2, char a3)
{
  void **v4; // rdi
  void *v5; // r10
  __int64 v6; // r9
  unsigned __int64 v7; // rcx
  __int64 v8; // rcx
  unsigned __int64 v9; // rsi
  __int64 v10; // rax
  __int64 v11; // rcx
  __int64 v12; // rax
  __m128i v13; // xmm1
  void *v14; // rcx
  __int64 v15; // rbx
  __int64 v16; // rax
  double v17; // xmm12_8
  __int64 v18; // rax
  __int64 v19; // rax
  double v20; // xmm6_8
  __int64 v21; // xmm11_8
  void *v22; // rcx
  __int64 v23; // rax
  __int64 v24; // rax
  __int64 v25; // xmm10_8
  void *v26; // rcx
  __int64 v27; // rax
  __int64 v28; // rax
  __int64 v29; // xmm9_8
  void *v30; // rcx
  __int64 v31; // rax
  __int64 v32; // rax
  __int64 v33; // xmm8_8
  void *v34; // rcx
  __int64 v35; // rax
  __int64 v36; // rax
  __int64 v37; // rcx
  __int64 v38; // xmm7_8
  void *v39; // rcx
  __int64 v40; // rbx
  __int128 *v41; // rdx
  void *v42; // rcx
  __int64 v43; // rax
  __int64 v44; // r9
  __int64 v45; // rcx
  __int64 v46; // rax
  void *v47; // rcx
  __int64 v48; // rbx
  __int64 v49; // rax
  __int64 v50; // r8
  __int64 v51; // rax
  __int64 v52; // rax
  double v53; // xmm0_8
  void *v54; // rcx
  __int64 v55; // rax
  __int64 v56; // rax
  double v57; // xmm0_8
  void *v58; // rcx
  __int64 v59; // rax
  __int64 v60; // rax
  double v61; // xmm0_8
  void *v62; // rcx
  __int64 v63; // rax
  __int64 v64; // rax
  double v65; // xmm0_8
  void *v66; // rcx
  __int64 v67; // rax
  __int64 v68; // rax
  void *v69; // rcx
  void *v70; // rax
  __int64 v71; // rcx
  _QWORD *v72; // rax
  void *v73; // rcx
  void *v74; // rax
  __int64 v75; // rcx
  __int64 v76; // rbx
  __int64 v78; // rbx
  __int128 v79; // [rsp+48h] [rbp-C0h] BYREF
  __int64 v80; // [rsp+58h] [rbp-B0h]
  unsigned __int64 v81; // [rsp+60h] [rbp-A8h]
  _QWORD v82[3]; // [rsp+68h] [rbp-A0h] BYREF
  unsigned __int64 v83; // [rsp+80h] [rbp-88h]
  double v84; // [rsp+88h] [rbp-80h]
  __int128 v85; // [rsp+90h] [rbp-78h] BYREF
  __m128i v86; // [rsp+A0h] [rbp-68h]
  __int64 v87; // [rsp+B0h] [rbp-58h]
  __int64 v88; // [rsp+B8h] [rbp-50h]
  __int64 v89; // [rsp+C0h] [rbp-48h]
  __int64 v90; // [rsp+C8h] [rbp-40h]
  __int64 v91; // [rsp+D0h] [rbp-38h]
  double v92; // [rsp+D8h] [rbp-30h] BYREF
  __int128 v93; // [rsp+E0h] [rbp-28h] BYREF
  __int128 v94; // [rsp+F0h] [rbp-18h]
  double v95; // [rsp+100h] [rbp-8h]
  double v96; // [rsp+108h] [rbp+0h]
  double v97; // [rsp+110h] [rbp+8h]
  double v98; // [rsp+118h] [rbp+10h]
  double v99; // [rsp+120h] [rbp+18h]

  v4 = a2;
  v5 = a2[2];
  if ( (unsigned __int64)a2[3] > 0xF )
    a2 = (void **)*a2;
  v6 = 0xCBF29CE484222325uLL;
  v7 = 0LL;
  if ( v5 )
  {
    do
      v6 = 0x100000001B3LL * (*((unsigned __int8 *)a2 + v7++) ^ (unsigned __int64)v6);
    while ( v7 < (unsigned __int64)v5 );
  }
  if ( !*(_QWORD *)(sub_1A8DF0(v7, &v79, v4, v6) + 8) )
  {
    v85 = 0LL;
    v86.m128i_i64[0] = 0LL;
    v9 = 7LL;
    v86.m128i_i64[1] = 7LL;
    LOWORD(v85) = 0;
    v10 = sub_51FB0((__int64)&v79, (__int64)v4);
    v12 = sub_7AC570(v11, v82, v10, 0LL);
    if ( &v85 != (__int128 *)v12 )
    {
      v85 = *(_OWORD *)v12;
      v86 = *(__m128i *)(v12 + 16);
      v13 = v86;
      *(_QWORD *)(v12 + 16) = 0LL;
      *(_QWORD *)(v12 + 24) = 7LL;
      *(_WORD *)v12 = 0;
      v9 = _mm_srli_si128(v13, 8).m128i_u64[0];
    }
    if ( v83 > 7 )
    {
      v14 = (void *)v82[0];
      if ( 2 * v83 + 2 >= 0x1000 )
      {
        v14 = *(void **)(v82[0] - 8LL);
        if ( (unsigned __int64)(v82[0] - (_QWORD)v14 - 8LL) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v14);
    }
    v15 = xmmword_13B4A10;
    v16 = sub_51FB0((__int64)v82, (__int64)v4);
    v17 = (double)(int)sub_7AC9D0(v15, v16, 0LL);
    v84 = v17;
    v18 = sub_936540(xmmword_13B4A10 + 3400, v4);
    v80 = 7LL;
    v81 = 15LL;
    v79 = 0x74726F666D6F43uLL;
    v19 = sub_936540(v18, &v79);
    v20 = 0.0;
    if ( *(_DWORD *)(v19 + 168) == 2 )
      v21 = *(_QWORD *)(v19 + 88);
    else
      v21 = 0LL;
    v87 = v21;
    if ( v81 > 0xF )
    {
      v22 = (void *)v79;
      if ( v81 + 1 >= 0x1000 )
      {
        v22 = *(void **)(v79 - 8);
        if ( (unsigned __int64)(v79 - (_QWORD)v22 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v22);
    }
    v23 = sub_936540(xmmword_13B4A10 + 3400, v4);
    v80 = 11LL;
    v81 = 15LL;
    *(_QWORD *)&v79 = 0x74616C756D697453LL;
    *((_QWORD *)&v79 + 1) = 7237481LL;
    v24 = sub_936540(v23, &v79);
    if ( *(_DWORD *)(v24 + 168) == 2 )
      v25 = *(_QWORD *)(v24 + 88);
    else
      v25 = 0LL;
    v88 = v25;
    if ( v81 > 0xF )
    {
      v26 = (void *)v79;
      if ( v81 + 1 >= 0x1000 )
      {
        v26 = *(void **)(v79 - 8);
        if ( (unsigned __int64)(v79 - (_QWORD)v26 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v26);
    }
    v27 = sub_936540(xmmword_13B4A10 + 3400, v4);
    v79 = 0LL;
    v80 = 6LL;
    v81 = 15LL;
    strcpy((char *)&v79, "Appeal");
    v28 = sub_936540(v27, &v79);
    if ( *(_DWORD *)(v28 + 168) == 2 )
      v29 = *(_QWORD *)(v28 + 88);
    else
      v29 = 0LL;
    v89 = v29;
    if ( v81 > 0xF )
    {
      v30 = (void *)v79;
      if ( v81 + 1 >= 0x1000 )
      {
        v30 = *(void **)(v79 - 8);
        if ( (unsigned __int64)(v79 - (_QWORD)v30 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v30);
    }
    v31 = sub_936540(xmmword_13B4A10 + 3400, v4);
    v79 = 0LL;
    v80 = 6LL;
    v81 = 15LL;
    strcpy((char *)&v79, "Health");
    v32 = sub_936540(v31, &v79);
    if ( *(_DWORD *)(v32 + 168) == 2 )
      v33 = *(_QWORD *)(v32 + 88);
    else
      v33 = 0LL;
    v90 = v33;
    if ( v81 > 0xF )
    {
      v34 = (void *)v79;
      if ( v81 + 1 >= 0x1000 )
      {
        v34 = *(void **)(v79 - 8);
        if ( (unsigned __int64)(v79 - (_QWORD)v34 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v34);
    }
    v35 = sub_936540(xmmword_13B4A10 + 3400, v4);
    *((_QWORD *)&v79 + 1) = 110LL;
    v80 = 9LL;
    v81 = 15LL;
    *(_QWORD *)&v79 = 0x6F6974756C6F7645LL;
    v36 = sub_936540(v35, &v79);
    if ( *(_DWORD *)(v36 + 168) == 2 )
      v38 = *(_QWORD *)(v36 + 88);
    else
      v38 = 0LL;
    v91 = v38;
    if ( v81 > 0xF )
    {
      v39 = (void *)v79;
      if ( v81 + 1 >= 0x1000 )
      {
        v39 = *(void **)(v79 - 8);
        if ( (unsigned __int64)(v79 - (_QWORD)v39 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v39);
    }
    v40 = sub_1A7B90(v37, v4);
    *(double *)v40 = v17;
    if ( (__int128 *)(v40 + 8) != &v85 )
    {
      v41 = &v85;
      if ( v9 > 7 )
        v41 = (__int128 *)v85;
      sub_5ABC0((void *)(v40 + 8), v41);
      v38 = v91;
      v33 = v90;
      v29 = v89;
      v25 = v88;
      v21 = v87;
      v9 = v86.m128i_u64[1];
    }
    *(_QWORD *)(v40 + 40) = v21;
    *(_QWORD *)(v40 + 48) = v25;
    *(_QWORD *)(v40 + 56) = v29;
    *(_QWORD *)(v40 + 64) = v33;
    *(_QWORD *)(v40 + 72) = v38;
    if ( v9 > 7 )
    {
      v42 = (void *)v85;
      if ( 2 * v9 + 2 >= 0x1000 )
      {
        v42 = *(void **)(v85 - 8);
        if ( (unsigned __int64)(v85 - (_QWORD)v42 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v42);
    }
    v93 = 0LL;
    *(_QWORD *)&v94 = 0LL;
    *((_QWORD *)&v94 + 1) = 7LL;
    LOWORD(v93) = 0;
    v43 = sub_51FB0((__int64)&v79, (__int64)v4);
    LOBYTE(v44) = 1;
    v46 = sub_7AC570(v45, v82, v43, v44);
    if ( &v93 != (__int128 *)v46 )
    {
      v93 = *(_OWORD *)v46;
      v94 = *(_OWORD *)(v46 + 16);
      *(_QWORD *)(v46 + 16) = 0LL;
      *(_QWORD *)(v46 + 24) = 7LL;
      *(_WORD *)v46 = 0;
    }
    if ( v83 > 7 )
    {
      v47 = (void *)v82[0];
      if ( 2 * v83 + 2 >= 0x1000 )
      {
        v47 = *(void **)(v82[0] - 8LL);
        if ( (unsigned __int64)(v82[0] - (_QWORD)v47 - 8LL) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v47);
    }
    v48 = xmmword_13B4A10;
    v49 = sub_51FB0((__int64)v82, (__int64)v4);
    LOBYTE(v50) = 1;
    v92 = (double)(int)sub_7AC9D0(v48, v49, v50);
    v51 = sub_936540(xmmword_13B4A10 + 3400, v4);
    v80 = 7LL;
    v81 = 15LL;
    v79 = 0x74726F666D6F43uLL;
    v52 = sub_936540(v51, &v79);
    if ( *(_DWORD *)(v52 + 168) == 2 )
      v53 = *(double *)(v52 + 88);
    else
      v53 = 0.0;
    v95 = v53 + v53;
    if ( v81 > 0xF )
    {
      v54 = (void *)v79;
      if ( v81 + 1 >= 0x1000 )
      {
        v54 = *(void **)(v79 - 8);
        if ( (unsigned __int64)(v79 - (_QWORD)v54 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v54);
    }
    v55 = sub_936540(xmmword_13B4A10 + 3400, v4);
    v80 = 11LL;
    v81 = 15LL;
    *(_QWORD *)&v79 = 0x74616C756D697453LL;
    *((_QWORD *)&v79 + 1) = 7237481LL;
    v56 = sub_936540(v55, &v79);
    if ( *(_DWORD *)(v56 + 168) == 2 )
      v57 = *(double *)(v56 + 88);
    else
      v57 = 0.0;
    v96 = v57 + v57;
    if ( v81 > 0xF )
    {
      v58 = (void *)v79;
      if ( v81 + 1 >= 0x1000 )
      {
        v58 = *(void **)(v79 - 8);
        if ( (unsigned __int64)(v79 - (_QWORD)v58 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v58);
    }
    v59 = sub_936540(xmmword_13B4A10 + 3400, v4);
    v79 = 0LL;
    v80 = 6LL;
    v81 = 15LL;
    strcpy((char *)&v79, "Appeal");
    v60 = sub_936540(v59, &v79);
    if ( *(_DWORD *)(v60 + 168) == 2 )
      v61 = *(double *)(v60 + 88);
    else
      v61 = 0.0;
    v97 = v61 + v61;
    if ( v81 > 0xF )
    {
      v62 = (void *)v79;
      if ( v81 + 1 >= 0x1000 )
      {
        v62 = *(void **)(v79 - 8);
        if ( (unsigned __int64)(v79 - (_QWORD)v62 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v62);
    }
    v63 = sub_936540(xmmword_13B4A10 + 3400, v4);
    v79 = 0LL;
    v80 = 6LL;
    v81 = 15LL;
    strcpy((char *)&v79, "Health");
    v64 = sub_936540(v63, &v79);
    if ( *(_DWORD *)(v64 + 168) == 2 )
      v65 = *(double *)(v64 + 88);
    else
      v65 = 0.0;
    v98 = v65 + v65;
    if ( v81 > 0xF )
    {
      v66 = (void *)v79;
      if ( v81 + 1 >= 0x1000 )
      {
        v66 = *(void **)(v79 - 8);
        if ( (unsigned __int64)(v79 - (_QWORD)v66 - 8) > 0x1F )
          invalid_parameter_noinfo_noreturn();
      }
      j_j_free(v66);
    }
    v67 = sub_936540(xmmword_13B4A10 + 3400, v4);
    *((_QWORD *)&v79 + 1) = 110LL;
    v80 = 9LL;
    v81 = 15LL;
    *(_QWORD *)&v79 = 0x6F6974756C6F7645LL;
    v68 = sub_936540(v67, &v79);
    if ( *(_DWORD *)(v68 + 168) == 2 )
      v20 = *(double *)(v68 + 88);
    v99 = v20 + v20;
    if ( v81 > 0xF )
      std::string::_Deallocate_for_capacity(&v79, v79);
    v69 = v4[2];
    if ( (unsigned __int64)(0x7FFFFFFFFFFFFFFFLL - (_QWORD)v69) < 5 )
      unknown_libname_4(v69);
    v70 = v4;
    if ( (unsigned __int64)v4[3] > 0xF )
      v70 = *v4;
    sub_51850(v82, 5uLL, v70, (size_t)v4[2]);
    v72 = (_QWORD *)sub_1A84F0(v71, &v79, v82);
    sub_1A1DB0(*v72 + 48LL, &v92);
    if ( v83 > 0xF )
      std::string::_Deallocate_for_capacity(v82, v82[0]);
    if ( *((_QWORD *)&v94 + 1) > 7uLL )
      std::wstring::_Deallocate_for_capacity(&v93, v93);
  }
  if ( a3 )
  {
    v73 = v4[2];
    if ( (unsigned __int64)(0x7FFFFFFFFFFFFFFFLL - (_QWORD)v73) < 5 )
      unknown_libname_4(v73);
    v74 = v4;
    if ( (unsigned __int64)v4[3] > 0xF )
      v74 = *v4;
    sub_51850(v82, 5uLL, v74, (size_t)v4[2]);
    v76 = *(_QWORD *)sub_1A84F0(v75, &v79, v82);
    sub_522D0((__int64)v82);
    sub_522D0((__int64)v4);
    return v76 + 48;
  }
  else
  {
    v78 = sub_1A7B90(v8, v4);                   // CALLSTACK HIT
                                                // Address: 0x1A1D01
                                                // Nearest statement EA: 0x1A1D01
                                                // Delta: +0 bytes
                                                // Instruction:
                                                // mov     rbx, rax
    sub_522D0((__int64)v4);
    return v78;
  }
}



########################################################################################################################
Function: sub_1A1E20
Start EA: 0x1A1E20
########################################################################################################################

Callstack addresses found:
    0x1A23EF

__int64 __fastcall sub_1A1E20(__int64 a1)
{
  unsigned int v2; // ebx
  int v3; // eax
  void **v4; // rsi
  void *v5; // rcx
  __int64 v6; // rcx
  __int64 v7; // rax
  __int64 v8; // rax
  __int64 v9; // rbx
  __int64 v10; // rax
  __int64 v11; // rax
  _QWORD *v12; // rdx
  __int64 v13; // rdx
  _QWORD *v14; // rdx
  __int64 v15; // rcx
  __int64 v16; // rax
  __int64 v17; // rbx
  __int64 v18; // rax
  __int64 v19; // rax
  __int64 v20; // rbx
  __int64 v21; // rax
  __int64 v22; // rax
  __int64 v23; // rbx
  __int64 v24; // rax
  __int64 v25; // rax
  __int64 v26; // rbx
  __int64 v27; // rdx
  __int64 v28; // r8
  __int64 result; // rax
  __int128 v30; // [rsp+20h] [rbp-E0h] BYREF
  __int64 v31; // [rsp+30h] [rbp-D0h]
  __int64 v32; // [rsp+38h] [rbp-C8h]
  __int128 v33; // [rsp+40h] [rbp-C0h] BYREF
  __int64 v34; // [rsp+50h] [rbp-B0h]
  __int64 v35; // [rsp+58h] [rbp-A8h]
  __int64 (__fastcall **v36)(); // [rsp+60h] [rbp-A0h] BYREF
  __int64 v37; // [rsp+68h] [rbp-98h]
  __int64 (__fastcall ***v38)(); // [rsp+98h] [rbp-68h]
  _QWORD v39[7]; // [rsp+A0h] [rbp-60h] BYREF
  _QWORD *v40; // [rsp+D8h] [rbp-28h]
  _QWORD v41[7]; // [rsp+E0h] [rbp-20h] BYREF
  _QWORD *v42; // [rsp+118h] [rbp+18h]

  v2 = *(_DWORD *)(a1 + 60);
  v3 = *(_DWORD *)(a1 + 56);
  if ( v2 == v3 )
  {
    v2 = (int)((double)v3 * 1.5);
    v4 = (void **)(a1 + 64);
    v5 = *(void **)(a1 + 64);
    if ( v2 < 2 )
      v2 = 2;
    *v4 = j__realloc_base(v5, 4LL * v2);
    *(_DWORD *)(a1 + 56) = v2;
    if ( v2 >= *(_DWORD *)(a1 + 60) )
      v2 = *(_DWORD *)(a1 + 60);
    else
      *(_DWORD *)(a1 + 60) = v2;
  }
  *(_DWORD *)(*(_QWORD *)(a1 + 64) + 4LL * v2) = 1;
  ++*(_DWORD *)(a1 + 60);
  v6 = *(_QWORD *)(a1 + 24);
  if ( *(_BYTE *)(v6 + 24) )
    v7 = 0LL;
  else
    v7 = sub_1A9B30(*(_QWORD *)(v6 + 8), *(_QWORD *)(a1 + 24));
  *(_QWORD *)(a1 + 96) = v7;
  v8 = *(_QWORD *)(a1 + 24);
  v9 = *(_QWORD *)(v8 + 8);
  if ( *(_BYTE *)(v9 + 1200) )
  {
    v10 = 0LL;
  }
  else
  {
    v11 = sub_95AFE0(*(_QWORD *)(v8 + 8));
    v10 = sub_1AA8F0(v9, v11);
  }
  *(_QWORD *)(a1 + 104) = v10;
  v39[0] = ___7___Func_impl_no_alloc_V_lambda_1___1__init_FurnitureBuildingUI_glaiel__QEAAXXZ_X__V_std__6B_;
  v39[1] = a1;
  v40 = v39;
  sub_48C80(v39, v10 + 272);
  if ( v40 )
  {
    v12 = v39;
    LOBYTE(v12) = v40 != v39;
    (*(void (__fastcall **)(_QWORD *, _QWORD *))(*v40 + 32LL))(v40, v12);
  }
  v13 = *(_QWORD *)(a1 + 104) + 208LL;
  v41[0] = ___7___Func_impl_no_alloc_V_lambda_2___1__init_FurnitureBuildingUI_glaiel__QEAAXXZ_X__V_std__6B_;
  v41[1] = a1;
  v42 = v41;
  sub_48C80(v41, v13);
  if ( v42 )
  {
    v14 = v41;
    LOBYTE(v14) = v42 != v41;
    (*(void (__fastcall **)(_QWORD *, _QWORD *))(*v42 + 32LL))(v42, v14);
  }
  v15 = *(_QWORD *)(a1 + 104);
  *(_QWORD *)(a1 + 112) = *(_QWORD *)(v15 + 80);
  *(_BYTE *)(v15 + 200) = 1;
  *(_BYTE *)(*(_QWORD *)(a1 + 104) + 202LL) = 1;
  sub_51C20((void *)(*(_QWORD *)(a1 + 104) + 168LL), &qword_ED8DB8, 9uLL);
  v16 = *(_QWORD *)(a1 + 24);
  v17 = *(_QWORD *)(v16 + 8);
  if ( !*(_BYTE *)(v17 + 1200) )
  {
    v18 = sub_95AFE0(*(_QWORD *)(v16 + 8));
    sub_1AAAB0(v17, v18);
  }
  v19 = *(_QWORD *)(a1 + 24);
  v20 = *(_QWORD *)(v19 + 8);
  if ( !*(_BYTE *)(v20 + 1200) )
  {
    v21 = sub_95AFE0(*(_QWORD *)(v19 + 8));
    sub_1AACE0(v20, v21);
  }
  v22 = *(_QWORD *)(a1 + 24);
  v23 = *(_QWORD *)(v22 + 8);
  if ( !*(_BYTE *)(v23 + 1200) )
  {
    v24 = sub_95AFE0(*(_QWORD *)(v22 + 8));
    sub_1AAFD0(v23, v24);
  }
  v25 = sub_EB750(*(_QWORD *)(a1 + 24));
  sub_201570(v25);
  v26 = sub_54150(a1);
  v36 = ___7___Func_impl_no_alloc_V_lambda_3___1__init_FurnitureBuildingUI_glaiel__QEAAXXZ_X__V_std__6B_;
  v37 = a1;
  v38 = &v36;
  v33 = 0LL;
  v34 = 0LL;
  v35 = 15LL;
  LOBYTE(v33) = 0;
  *((_QWORD *)&v30 + 1) = 116LL;
  v31 = 9LL;
  v32 = 15LL;
  *(_QWORD *)&v30 = 0x66656C5F65676170LL;
  *(_QWORD *)(sub_96BC70(*(_QWORD *)(a1 + 112), &v30, &v33, &v36) + 136) = v26 + 8584;
  v36 = ___7___Func_impl_no_alloc_V_lambda_4___1__init_FurnitureBuildingUI_glaiel__QEAAXXZ_X__V_std__6B_;
  v37 = a1;
  v38 = &v36;
  v33 = 0LL;
  v34 = 0LL;
  v35 = 15LL;
  LOBYTE(v33) = 0;
  *((_QWORD *)&v30 + 1) = 29800LL;
  v31 = 10LL;
  v32 = 15LL;
  *(_QWORD *)&v30 = 0x6769725F65676170LL;
  *(_QWORD *)(sub_96BC70(*(_QWORD *)(a1 + 112), &v30, &v33, &v36) + 136) = v26 + 8744;
  v36 = ___7___Func_impl_no_alloc_V_lambda_5___1__init_FurnitureBuildingUI_glaiel__QEAAXXZ_X__V_std__6B_;
  v37 = a1;
  v38 = &v36;
  v30 = 0LL;
  v31 = 0LL;
  v32 = 15LL;
  LOBYTE(v30) = 0;
  v34 = 8LL;
  v35 = 15LL;
  v33 = 0x3332315F74726F73uLL;
  sub_96BC70(*(_QWORD *)(a1 + 112), &v33, &v30, &v36);
  v36 = ___7___Func_impl_no_alloc_V_lambda_6___1__init_FurnitureBuildingUI_glaiel__QEAAXXZ_X__V_std__6B_;
  v37 = a1;
  v38 = &v36;
  v30 = 0LL;
  v31 = 0LL;
  v32 = 15LL;
  LOBYTE(v30) = 0;
  v34 = 8LL;
  v35 = 15LL;
  v33 = 0x6362615F74726F73uLL;
  sub_96BC70(*(_QWORD *)(a1 + 112), &v33, &v30, &v36);
  v36 = ___7___Func_impl_no_alloc_V_lambda_7___1__init_FurnitureBuildingUI_glaiel__QEAAXXZ_X__V_std__6B_;
  v37 = a1;
  v38 = &v36;
  v33 = 0LL;
  v34 = 0LL;
  v35 = 15LL;
  LOBYTE(v33) = 0;
  *((_QWORD *)&v30 + 1) = 25973LL;
  v31 = 10LL;
  v32 = 15LL;
  *(_QWORD *)&v30 = 0x6C61765F74726F73LL;
  sub_96BC70(*(_QWORD *)(a1 + 112), &v30, &v33, &v36);
  v36 = ___7___Func_impl_no_alloc_V_lambda_8___1__init_FurnitureBuildingUI_glaiel__QEAAXXZ_X__V_std__6B_;
  v37 = a1;
  v38 = &v36;
  v31 = 13LL;
  v32 = 15LL;
  strcpy((char *)&v30, "[img:comfort]");
  HIWORD(v30) = 0;
  *((_QWORD *)&v33 + 1) = 116LL;
  v34 = 9LL;
  v35 = 15LL;
  *(_QWORD *)&v33 = 0x6174735F74726F73LL;
  sub_96BC70(*(_QWORD *)(a1 + 112), &v33, &v30, &v36);
  sub_1A2B50(a1, v27, v28);
  sub_1A4A00(a1, *(unsigned int *)(a1 + 124));  // CALLSTACK HIT
                                                // Address: 0x1A23EF
                                                // Nearest statement EA: 0x1A23EA
                                                // Delta: +5 bytes
                                                // Instruction:
                                                // mov     rax, cs:qword_13C7BD0
  result = *(_QWORD *)(a1 + 104);
  *(_BYTE *)(result + 204) = *(_BYTE *)(*(_QWORD *)(qword_13C7BD0 + 1448) + 632LL) == 0;
  *(_BYTE *)(a1 + 88) = 1;
  return result;
}



########################################################################################################################
Function: sub_1A4310
Start EA: 0x1A4310
########################################################################################################################

Callstack addresses found:
    0x1A44B4

void __fastcall sub_1A4310(__int64 a1)
{
  unsigned __int64 **v2; // rsi
  unsigned __int64 **i; // rbp
  unsigned __int64 *v4; // r14
  unsigned __int64 v5; // r8
  __int64 v6; // rax
  unsigned __int64 v7; // rdx
  _DWORD *v8; // rcx
  __int64 v9; // rcx
  __int64 *v10; // rcx
  unsigned __int64 v11; // rdx
  __int64 *v12; // rax
  int v13; // eax
  unsigned int v14; // ebx
  unsigned int v15; // eax

  if ( *(_BYTE *)(a1 + 88) )
  {
    *(_BYTE *)(a1 + 88) = 0;
    *(_DWORD *)(a1 + 76) = 0;
    v2 = *(unsigned __int64 ***)(qword_13C7940 + 64);
    for ( i = &v2[*(unsigned int *)(qword_13C7940 + 60)]; v2 != i; ++v2 )
    {
      v4 = *v2;
      v5 = (*v2)[4];
      v6 = (__int64)(*v2 + 1);
      v7 = (*v2)[3];
      v8 = (_DWORD *)v6;
      if ( v5 > 0xF )
        v8 = *(_DWORD **)v6;
      if ( v7 != 4 || *v8 != 1886351216 )
      {
        if ( v5 > 0xF )
          v6 = *(_QWORD *)v6;
        if ( v7 != 10 )
          goto LABEL_13;
        v9 = *(_QWORD *)v6 - 0x646565666F747561LL;
        if ( *(_QWORD *)v6 == 0x646565666F747561LL )
          v9 = *(unsigned __int16 *)(v6 + 8) - 29285LL;
        if ( v9 )
        {
LABEL_13:
          v10 = *(__int64 **)(a1 + 136);
          v11 = *v4;
          v12 = (__int64 *)v10[1];
          while ( !*((_BYTE *)v12 + 25) )
          {
            if ( v12[4] >= v11 )
            {
              v10 = v12;
              v12 = (__int64 *)*v12;
            }
            else
            {
              v12 = (__int64 *)v12[2];
            }
          }
          if ( *((_BYTE *)v10 + 25) || v11 < v10[4] )
          {
            v13 = *(_DWORD *)(a1 + 72);
            v14 = *(_DWORD *)(a1 + 76);
            if ( v14 == v13 )
            {
              v14 = (int)((double)v13 * 1.5);
              if ( v14 < 2 )
                v14 = 2;
              *(_QWORD *)(a1 + 80) = j__realloc_base(*(void **)(a1 + 80), 8LL * v14);
              v15 = *(_DWORD *)(a1 + 76);
              *(_DWORD *)(a1 + 72) = v14;
              if ( v14 >= v15 )
                v14 = v15;
              else
                *(_DWORD *)(a1 + 76) = v14;
            }
            *(_QWORD *)(*(_QWORD *)(a1 + 80) + 8LL * v14) = v4;
            ++*(_DWORD *)(a1 + 76);
          }
        }
      }
    }
    sub_1A8EE0(
      *(_QWORD *)(a1 + 80),
      *(_QWORD *)(a1 + 80) + 8LL * *(unsigned int *)(a1 + 76),
      *(unsigned int *)(a1 + 76),
      a1);                                      // CALLSTACK HIT
                                                // Address: 0x1A44B4
                                                // Nearest statement EA: 0x1A44AF
                                                // Delta: +5 bytes
                                                // Instruction:
                                                // mov     rsi, [rsp+48h+arg_10]
  }
}



########################################################################################################################
Function: sub_1A44D0
Start EA: 0x1A44D0
########################################################################################################################

Callstack addresses found:
    0x1A4540

__int64 __fastcall sub_1A44D0(__int64 a1, __int64 a2, __int64 a3)
{
  char v5; // bl
  void **v6; // rax
  __int64 v7; // rcx
  __int64 v8; // rbx
  __int64 v9; // r8
  double v10; // xmm6_8
  double v11; // xmm7_8
  double v12; // xmm8_8
  double v13; // xmm9_8
  double v14; // xmm10_8
  void **v15; // rax
  __int64 v16; // rcx
  __int64 v17; // rbx
  __int64 v18; // r8
  double v19; // xmm2_8
  double v20; // xmm3_8
  double v21; // xmm4_8
  double v22; // xmm5_8
  double v23; // xmm11_8
  int v24; // eax
  __int64 v25; // r11
  unsigned __int8 v26; // bl
  char *v28; // rax
  char *v29; // r10
  unsigned __int64 v30; // r9
  unsigned __int64 v31; // rcx
  signed __int64 v32; // r10
  unsigned __int16 v33; // dx
  bool v34; // cf
  char v35; // al
  int v36; // eax
  char *v37; // rax
  char *v38; // r8
  signed __int64 v39; // r8
  unsigned __int16 v40; // cx
  bool v41; // cf
  char v42; // al
  int v43; // eax
  char *v44; // rax
  char *v45; // r10
  unsigned __int64 v46; // r9
  unsigned __int64 v47; // rcx
  signed __int64 v48; // r10
  unsigned __int16 v49; // dx
  bool v50; // cf
  char v51; // al
  int v52; // eax
  char *v53; // rax
  char *v54; // r8
  signed __int64 v55; // r8
  unsigned __int16 v56; // cx
  bool v57; // cf
  char v58; // al
  int v59; // eax
  double v60; // [rsp+20h] [rbp-E0h]
  _QWORD v61[2]; // [rsp+28h] [rbp-D8h] BYREF
  unsigned __int64 v62; // [rsp+38h] [rbp-C8h]
  unsigned __int64 v63; // [rsp+40h] [rbp-C0h]
  double v64; // [rsp+48h] [rbp-B8h]
  double v65; // [rsp+50h] [rbp-B0h]
  double v66; // [rsp+58h] [rbp-A8h]
  double v67; // [rsp+60h] [rbp-A0h]
  double v68; // [rsp+68h] [rbp-98h]
  double v69; // [rsp+70h] [rbp-90h]
  _QWORD v70[2]; // [rsp+78h] [rbp-88h] BYREF
  unsigned __int64 v71; // [rsp+88h] [rbp-78h]
  unsigned __int64 v72; // [rsp+90h] [rbp-70h]
  double v73; // [rsp+98h] [rbp-68h]
  double v74; // [rsp+A0h] [rbp-60h]
  double v75; // [rsp+A8h] [rbp-58h]
  double v76; // [rsp+B0h] [rbp-50h]
  double v77; // [rsp+B8h] [rbp-48h]
  _BYTE v78[32]; // [rsp+C0h] [rbp-40h] BYREF
  _BYTE v79[128]; // [rsp+E0h] [rbp-20h] BYREF

  v5 = (*(_BYTE *)(a2 + 40) & 2) != 0;
  v6 = (void **)sub_51FB0((__int64)v78, a2 + 8);
  v8 = sub_1A11E0(v7, v6, v5);                  // CALLSTACK HIT
                                                // Address: 0x1A4540
                                                // Nearest statement EA: 0x1A4540
                                                // Delta: +0 bytes
                                                // Instruction:
                                                // mov     rbx, rax
  v69 = *(double *)v8;
  sub_5AAD0((__int64)v70, (_QWORD *)(v8 + 8), v9);
  v10 = *(double *)(v8 + 40);
  v73 = v10;
  v11 = *(double *)(v8 + 48);
  v74 = v11;
  v12 = *(double *)(v8 + 56);
  v75 = v12;
  v13 = *(double *)(v8 + 64);
  v76 = v13;
  v14 = *(double *)(v8 + 72);
  v77 = v14;
  LOBYTE(v8) = (*(_BYTE *)(a3 + 40) & 2) != 0;
  v15 = (void **)sub_51FB0((__int64)v79, a3 + 8);
  v17 = sub_1A11E0(v16, v15, v8);
  v60 = *(double *)v17;
  sub_5AAD0((__int64)v61, (_QWORD *)(v17 + 8), v18);
  v19 = *(double *)(v17 + 40);
  v64 = v19;
  v20 = *(double *)(v17 + 48);
  v65 = v20;
  v21 = *(double *)(v17 + 56);
  v66 = v21;
  v22 = *(double *)(v17 + 64);
  v67 = v22;
  v23 = *(double *)(v17 + 72);
  v68 = v23;
  v24 = *(_DWORD *)(*(_QWORD *)a1 + 60LL) - 1;
  if ( v24 >= 0 )
  {
    v25 = v24;
    while ( 1 )
    {
      switch ( *(_DWORD *)(*(_QWORD *)(*(_QWORD *)a1 + 64LL) + 4 * v25) )
      {
        case 0xFFFFFFF7:
          if ( v14 < v23 )
            goto LABEL_122;
          if ( v14 > v23 )
            goto LABEL_6;
          goto LABEL_47;
        case 0xFFFFFFF8:
          if ( v13 < v22 )
            goto LABEL_122;
          if ( v13 <= v22 )
            goto LABEL_47;
          goto LABEL_6;
        case 0xFFFFFFF9:
          if ( v12 < v21 )
            goto LABEL_122;
          if ( v12 <= v21 )
            goto LABEL_47;
          goto LABEL_6;
        case 0xFFFFFFFA:
          if ( v11 < v20 )
            goto LABEL_122;
          if ( v11 <= v20 )
            goto LABEL_47;
          goto LABEL_6;
        case 0xFFFFFFFB:
          if ( v10 < v19 )
            goto LABEL_122;
          if ( v10 <= v19 )
            goto LABEL_47;
          goto LABEL_6;
        case 0xFFFFFFFD:
          if ( v69 < v60 )
            goto LABEL_122;
          if ( v69 <= v60 )
            goto LABEL_47;
          goto LABEL_6;
        case 0xFFFFFFFE:
          v44 = (char *)v61;
          if ( v63 > 7 )
            v44 = (char *)v61[0];
          v45 = (char *)v70;
          if ( v72 > 7 )
            v45 = (char *)v70[0];
          v46 = v71;
          if ( v62 < v71 )
            v46 = v62;
          v47 = v46;
          if ( !v46 )
            goto LABEL_59;
          v48 = v45 - v44;
          while ( 1 )
          {
            v49 = *(_WORD *)&v44[v48];
            v50 = v49 < *(_WORD *)v44;
            if ( v49 != *(_WORD *)v44 )
              break;
            v44 += 2;
            if ( !--v47 )
            {
LABEL_59:
              if ( v71 < v62 )
              {
                v51 = -1;
                goto LABEL_67;
              }
              if ( v71 > v62 )
              {
LABEL_66:
                v51 = 1;
                goto LABEL_67;
              }
              goto LABEL_68;
            }
          }
          v52 = 1;
          if ( v50 )
            v52 = -1;
          if ( v52 >= 0 )
            goto LABEL_66;
          v51 = -1;
LABEL_67:
          if ( v51 > 0 )
            goto LABEL_122;
LABEL_68:
          v53 = (char *)v61;
          if ( v63 > 7 )
            v53 = (char *)v61[0];
          v54 = (char *)v70;
          if ( v72 > 7 )
            v54 = (char *)v70[0];
          if ( v46 )
          {
            v55 = v54 - v53;
            while ( 1 )
            {
              v56 = *(_WORD *)&v53[v55];
              v57 = v56 < *(_WORD *)v53;
              if ( v56 != *(_WORD *)v53 )
                break;
              v53 += 2;
              if ( !--v46 )
                goto LABEL_76;
            }
            v59 = 1;
            if ( v57 )
              v59 = -1;
            if ( v59 < 0 )
            {
              v58 = -1;
              goto LABEL_84;
            }
          }
          else
          {
LABEL_76:
            if ( v71 < v62 )
            {
              v58 = -1;
              goto LABEL_84;
            }
            if ( v71 <= v62 )
              goto LABEL_47;
          }
          v58 = 1;
LABEL_84:
          if ( v58 < 0 )
            goto LABEL_6;
          goto LABEL_47;
        case 0xFFFFFFFF:
          if ( *(_QWORD *)a2 < *(_QWORD *)a3 )
            goto LABEL_122;
          if ( *(_QWORD *)a2 > *(_QWORD *)a3 )
            goto LABEL_6;
          goto LABEL_47;
        case 1:
          if ( *(_QWORD *)a2 > *(_QWORD *)a3 )
            goto LABEL_122;
          if ( *(_QWORD *)a2 >= *(_QWORD *)a3 )
            goto LABEL_47;
          goto LABEL_6;
        case 2:
          v28 = (char *)v61;
          if ( v63 > 7 )
            v28 = (char *)v61[0];
          v29 = (char *)v70;
          if ( v72 > 7 )
            v29 = (char *)v70[0];
          v30 = v71;
          if ( v62 < v71 )
            v30 = v62;
          v31 = v30;
          if ( !v30 )
            goto LABEL_21;
          v32 = v29 - v28;
          while ( 1 )
          {
            v33 = *(_WORD *)&v28[v32];
            v34 = v33 < *(_WORD *)v28;
            if ( v33 != *(_WORD *)v28 )
              break;
            v28 += 2;
            if ( !--v31 )
            {
LABEL_21:
              if ( v71 < v62 )
              {
                v35 = -1;
                goto LABEL_29;
              }
              if ( v71 > v62 )
              {
LABEL_28:
                v35 = 1;
                goto LABEL_29;
              }
              goto LABEL_30;
            }
          }
          v36 = 1;
          if ( v34 )
            v36 = -1;
          if ( v36 >= 0 )
            goto LABEL_28;
          v35 = -1;
LABEL_29:
          if ( v35 < 0 )
          {
LABEL_122:
            v26 = 1;
            goto LABEL_7;
          }
LABEL_30:
          v37 = (char *)v61;
          if ( v63 > 7 )
            v37 = (char *)v61[0];
          v38 = (char *)v70;
          if ( v72 > 7 )
            v38 = (char *)v70[0];
          if ( v30 )
          {
            v39 = v38 - v37;
            while ( 1 )
            {
              v40 = *(_WORD *)&v37[v39];
              v41 = v40 < *(_WORD *)v37;
              if ( v40 != *(_WORD *)v37 )
                break;
              v37 += 2;
              if ( !--v30 )
                goto LABEL_38;
            }
            v43 = 1;
            if ( v41 )
              v43 = -1;
            if ( v43 >= 0 )
LABEL_45:
              v42 = 1;
            else
              v42 = -1;
LABEL_46:
            if ( v42 > 0 )
              goto LABEL_6;
            goto LABEL_47;
          }
LABEL_38:
          if ( v71 < v62 )
          {
            v42 = -1;
            goto LABEL_46;
          }
          if ( v71 > v62 )
            goto LABEL_45;
LABEL_47:
          if ( --v25 < 0 )
            goto LABEL_6;
          break;
        case 3:
          if ( v69 > v60 )
            goto LABEL_122;
          if ( v69 >= v60 )
            goto LABEL_47;
          goto LABEL_6;
        case 5:
          if ( v10 > v19 )
            goto LABEL_122;
          if ( v10 >= v19 )
            goto LABEL_47;
          goto LABEL_6;
        case 6:
          if ( v11 > v20 )
            goto LABEL_122;
          if ( v11 >= v20 )
            goto LABEL_47;
          goto LABEL_6;
        case 7:
          if ( v12 > v21 )
            goto LABEL_122;
          if ( v12 >= v21 )
            goto LABEL_47;
          goto LABEL_6;
        case 8:
          if ( v13 > v22 )
            goto LABEL_122;
          if ( v13 >= v22 )
            goto LABEL_47;
          goto LABEL_6;
        case 9:
          if ( v14 > v23 )
            goto LABEL_122;
          if ( v14 >= v23 )
            goto LABEL_47;
          goto LABEL_6;
        default:
          goto LABEL_47;
      }
    }
  }
LABEL_6:
  v26 = 0;
LABEL_7:
  sub_51DE0(v61);
  sub_51DE0(v70);
  return v26;
}



########################################################################################################################
Function: sub_1A4A00
Start EA: 0x1A4A00
########################################################################################################################

Callstack addresses found:
    0x1A4A40

__int64 __fastcall sub_1A4A00(__int64 a1, int a2)
{
  int v4; // r12d
  _QWORD *v5; // rdi
  _QWORD *v6; // rbx
  _QWORD *v7; // rax
  __int64 v8; // rbx
  void **v9; // rcx
  void **v10; // rbx
  void **v11; // rax
  __int64 *v12; // rcx
  __int64 *v13; // rsi
  __int64 *v14; // rdx
  __int64 v15; // rax
  __int64 *v16; // r15
  __int64 v17; // rdi
  __int64 v18; // rcx
  unsigned int v19; // r13d
  __int64 v20; // r15
  __int64 v21; // rsi
  __int64 v22; // rbx
  char v23; // di
  __int64 v24; // rax
  __int64 v25; // rsi
  __int64 v26; // rax
  __int64 v27; // rbx
  __int64 v28; // rax
  unsigned __int8 *v29; // r11
  __int64 v30; // rax
  unsigned __int64 v31; // rdx
  __int64 v32; // r8
  _QWORD *v33; // rcx
  _QWORD *v34; // r10
  _QWORD *v35; // rax
  __int64 v36; // rdx
  __m128d v37; // xmm1
  __int64 v38; // rcx
  __m128d v39; // xmm0
  __int64 v40; // rax
  _QWORD *v41; // rcx
  __int64 v42; // rdx
  __int64 v43; // rax
  __m128d v44; // xmm2
  __m128d v45; // xmm1
  __int64 v46; // rdx
  _QWORD *v47; // rdi
  _QWORD *i; // rbx
  __int64 v49; // rcx
  __int64 v51; // [rsp+28h] [rbp-99h] BYREF
  _QWORD *v52; // [rsp+30h] [rbp-91h] BYREF
  __int64 v53; // [rsp+38h] [rbp-89h]
  _QWORD v54[3]; // [rsp+40h] [rbp-81h] BYREF
  __int64 v55; // [rsp+58h] [rbp-69h]
  __int64 v56; // [rsp+60h] [rbp-61h]
  __int128 v57; // [rsp+68h] [rbp-59h] BYREF
  double v58; // [rsp+78h] [rbp-49h]
  double v59; // [rsp+80h] [rbp-41h]
  __int64 v60; // [rsp+88h] [rbp-39h]
  _BYTE v61[88]; // [rsp+90h] [rbp-31h] BYREF
  __int64 v63; // [rsp+138h] [rbp+77h]
  __int64 v64; // [rsp+140h] [rbp+7Fh] BYREF

  v4 = 0;
  sub_1A4310(a1);                               // CALLSTACK HIT
                                                // Address: 0x1A4A40
                                                // Nearest statement EA: 0x1A4A3B
                                                // Delta: +5 bytes
                                                // Instruction:
                                                // mov     dword ptr [rsp+110h+var_F0], r12d
  v53 = 0LL;
  v5 = operator new(0x20uLL);
  *v5 = v5;
  v5[1] = v5;
  v52 = v5;
  v55 = 7LL;
  v56 = 8LL;
  LODWORD(v51) = 1065353216;
  v6 = operator new(0x80uLL);
  v54[0] = v6;
  v7 = v6 + 16;
  v54[1] = v6 + 16;
  v54[2] = v6 + 16;
  do
    *v6++ = v5;
  while ( v6 != v7 );
  v8 = *(_QWORD *)(*(_QWORD *)(a1 + 24) + 8LL);
  sub_95B070(v8, 1003LL);
  v9 = *(void ***)(*(_QWORD *)(v8 + 32) + 16048LL);
  v10 = 0LL;
  v11 = 0LL;
  if ( v9 )
  {
    ++*(_DWORD *)v9;
    v10 = v9;
    v11 = v9;
  }
  if ( v11 )
  {
    v12 = (__int64 *)v11[2];
    v13 = v12;
    v14 = v12;
    v15 = *((unsigned int *)v11 + 3);
  }
  else
  {
    v12 = 0LL;
    v13 = 0LL;
    v14 = 0LL;
    v15 = 0LL;
  }
  v16 = &v14[v15];
  if ( v12 != v16 )
  {
    do
    {
      v17 = *v13;
      v64 = *(_QWORD *)(*v13 + 72);
      *(_QWORD *)(*(_QWORD *)sub_1A8280(&v51, v61, &v64) + 24LL) = v17;
      ++v13;
    }
    while ( v13 != v16 );
  }
  if ( v10 )
  {
    if ( (int)--*(_DWORD *)v10 <= 0 )
    {
      free(v10[2]);
      j_j_free(v10);
    }
  }
  v18 = 0LL;
  v63 = 0LL;
  v19 = 21 * a2;
  v20 = 168LL * a2;
  v21 = 21LL * a2;
  v64 = v21;
  do
  {
    if ( v21 < 0 || v19 >= *(_DWORD *)(a1 + 76) )
      goto LABEL_45;
    v22 = *(_QWORD *)(*(_QWORD *)sub_1A8280(&v51, v61, *(_QWORD *)(v20 + *(_QWORD *)(a1 + 80))) + 24LL);
    v23 = 0;
    v24 = *(_QWORD *)(a1 + 80);
    if ( v22 )
    {
      v29 = *(unsigned __int8 **)(v20 + v24);
      v30 = v29[7];
      v31 = 2
          * ((0x100000001B3LL
            * (v30 ^ (0x100000001B3LL
                    * (v29[6] ^ (0x100000001B3LL
                               * (v29[5] ^ (0x100000001B3LL
                                          * (v29[4] ^ (0x100000001B3LL
                                                     * (v29[3] ^ (0x100000001B3LL
                                                                * (v29[2] ^ (0x100000001B3LL
                                                                           * (v29[1] ^ (0x100000001B3LL
                                                                                      * (*v29 ^ 0xCBF29CE484222325uLL)))))))))))))))) & v55);
      v32 = v54[0];
      v33 = *(_QWORD **)(v54[0]
                       + 16
                       * ((0x100000001B3LL
                         * (v30 ^ (0x100000001B3LL
                                 * (v29[6] ^ (0x100000001B3LL
                                            * (v29[5] ^ (0x100000001B3LL
                                                       * (v29[4] ^ (0x100000001B3LL
                                                                  * (v29[3] ^ (0x100000001B3LL
                                                                             * (v29[2] ^ (0x100000001B3LL
                                                                                        * (v29[1] ^ (0x100000001B3LL * (*v29 ^ 0xCBF29CE484222325uLL)))))))))))))))) & v55)
                       + 8);
      v34 = v52;
      if ( v33 != v52 )
      {
        if ( *(_QWORD *)v29 == v33[2] )
        {
LABEL_27:
          if ( v33 )
          {
            v35 = *(_QWORD **)(v54[0]
                             + 16
                             * ((0x100000001B3LL
                               * (v30 ^ (0x100000001B3LL
                                       * (v29[6] ^ (0x100000001B3LL
                                                  * (v29[5] ^ (0x100000001B3LL
                                                             * (v29[4] ^ (0x100000001B3LL
                                                                        * (v29[3] ^ (0x100000001B3LL
                                                                                   * (v29[2] ^ (0x100000001B3LL
                                                                                              * (v29[1] ^ (0x100000001B3LL * (*v29 ^ 0xCBF29CE484222325uLL)))))))))))))))) & v55));
            if ( *(_QWORD **)(v54[0] + 8 * v31 + 8) == v33 )
            {
              if ( v35 == v33 )
                *(_QWORD *)(v54[0] + 8 * v31) = v52;
              else
                v34 = (_QWORD *)v33[1];
              *(_QWORD *)(v32 + 8 * v31 + 8) = v34;
            }
            else if ( v35 == v33 )
            {
              *(_QWORD *)(v54[0] + 8 * v31) = *v33;
            }
            v36 = *v33;
            --v53;
            *(_QWORD *)v33[1] = v36;
            *(_QWORD *)(v36 + 8) = v33[1];
            j_j_free(v33);
          }
          v23 = 1;
          goto LABEL_37;
        }
        while ( v33 != *(_QWORD **)(v54[0]
                                  + 16
                                  * ((0x100000001B3LL
                                    * (v30 ^ (0x100000001B3LL
                                            * (v29[6] ^ (0x100000001B3LL
                                                       * (v29[5] ^ (0x100000001B3LL
                                                                  * (v29[4] ^ (0x100000001B3LL
                                                                             * (v29[3] ^ (0x100000001B3LL
                                                                                        * (v29[2] ^ (0x100000001B3LL * (v29[1] ^ (0x100000001B3LL * (*v29 ^ 0xCBF29CE484222325uLL)))))))))))))))) & v55)) )
        {
          v33 = (_QWORD *)v33[1];
          if ( *(_QWORD *)v29 == v33[2] )
            goto LABEL_27;
        }
      }
      v33 = 0LL;
      goto LABEL_27;
    }
    v25 = *(_QWORD *)(v20 + v24);
    v26 = *(_QWORD *)(a1 + 24);
    v27 = *(_QWORD *)(v26 + 8);
    if ( *(_BYTE *)(v27 + 1200) )
    {
      v22 = 0LL;
    }
    else
    {
      v28 = sub_95AFE0(*(_QWORD *)(v26 + 8));
      v22 = sub_1AB2D0(v27, v28, v25);
    }
    v21 = v64;
LABEL_37:
    *(_DWORD *)(*(_QWORD *)(v22 + 56) + 80LL) = *(_DWORD *)(*(_QWORD *)(a1 + 112) + 112LL);
    v37 = (__m128d)COERCE_UNSIGNED_INT64((double)(int)(((unsigned int)(((unsigned __int64)(1431655765LL * v4) >> 32) - v4) >> 31)
                                                     + ((int)(((unsigned __int64)(1431655765LL * v4) >> 32) - v4) >> 1)));
    v37.m128d_f64[0] = v37.m128d_f64[0] * 3.0 + 9.0;
    *(double *)(v22 + 80) = (double)(v4 % 3) * 3.0 - 30.33333333333333;
    *(double *)(v22 + 88) = v37.m128d_f64[0];
    if ( !v23 )
    {
      v38 = *(_QWORD *)(*(_QWORD *)(a1 + 104) + 96LL);
      v37.m128d_f64[0] = v37.m128d_f64[0] + *(double *)(v38 + 136);
      v39 = (__m128d)*(unsigned __int64 *)(v38 + 128);
      v39.m128d_f64[0] = v39.m128d_f64[0] + *(double *)(v22 + 80);
      v40 = *(_QWORD *)(v22 + 64);
      *(__m128d *)(v40 + 128) = _mm_unpacklo_pd(v39, v37);
      v58 = 0.0;
      *(_QWORD *)(v40 + 144) = 0LL;
      v41 = *(_QWORD **)(a1 + 104);
      v42 = *(_QWORD *)(v22 + 64);
      v43 = v41[12];
      v44 = (__m128d)*(unsigned __int64 *)(v42 + 136);
      v44.m128d_f64[0] = v44.m128d_f64[0] - *(double *)(v43 + 136);
      v45 = (__m128d)*(unsigned __int64 *)(v42 + 128);
      v45.m128d_f64[0] = v45.m128d_f64[0] - *(double *)(v43 + 128);
      *(_QWORD *)&v57 = v42;
      if ( v42 )
        *((_QWORD *)&v57 + 1) = *(_QWORD *)(v42 - 8);
      else
        v57 = 0uLL;
      v58 = v45.m128d_f64[0];
      v59 = v44.m128d_f64[0];
      LOBYTE(v60) = 0;
      v46 = v41[15];
      if ( v46 == v41[16] )
      {
        sub_202980(v41 + 14, v46, &v57);
      }
      else
      {
        *(_OWORD *)v46 = v57;
        *(__m128d *)(v46 + 16) = _mm_unpacklo_pd(v45, v44);
        *(_QWORD *)(v46 + 32) = v60;
        v41[15] += 40LL;
      }
    }
    ++v4;
    v18 = v63;
LABEL_45:
    ++v19;
    v63 = ++v18;
    v20 += 8LL;
    v64 = ++v21;
  }
  while ( v18 < 21 );
  v47 = v52;
  for ( i = (_QWORD *)*v52; i != v47; i = (_QWORD *)*i )
  {
    v49 = i[3];
    if ( v49 )
      sub_942970(v49);
  }
  sub_50940(v54);
  return sub_17FFF0(&v52);
}



########################################################################################################################
Function: sub_1A7B90
Start EA: 0x1A7B90
########################################################################################################################

Callstack addresses found:
    0x1A7B90

__int64 __fastcall sub_1A7B90(__int64 a1, _QWORD *a2)
{
  __int64 v2; // rdi
  unsigned __int64 v3; // r8
  __int64 v4; // rbx
  unsigned __int64 i; // rcx
  __int64 v6; // rax
  char *v7; // rbp
  unsigned __int64 v8; // rcx
  float v9; // xmm1_4
  __int64 v10; // rcx
  float v11; // xmm0_4
  __int64 v12; // rcx
  __int128 v14; // [rsp+20h] [rbp-28h] BYREF
  void **v15; // [rsp+30h] [rbp-18h] BYREF
  char *v16; // [rsp+38h] [rbp-10h]

  v2 = (__int64)a2;                             // CALLSTACK HIT
                                                // Address: 0x1A7B90
                                                // Nearest statement EA: 0x1A7BAA
                                                // Delta: -26 bytes
                                                // Instruction:
                                                // mov     [rsp+arg_0], rbx
  v3 = a2[2];
  if ( a2[3] > 0xFuLL )
    a2 = (_QWORD *)*a2;
  v4 = 0xCBF29CE484222325uLL;
  for ( i = 0LL; i < v3; ++i )
    v4 = 0x100000001B3LL * (*((unsigned __int8 *)a2 + i) ^ (unsigned __int64)v4);
  sub_1A8DF0(i, &v14, v2, v4);
  v6 = *((_QWORD *)&v14 + 1);
  if ( !*((_QWORD *)&v14 + 1) )
  {
    if ( qword_12E9D60 == 0x1FFFFFFFFFFFFFFLL )
      sub_D081C4("unordered_map/set too long");
    v15 = &qword_12E9D58;
    v16 = 0LL;
    v7 = (char *)operator new(0x80uLL);
    v16 = v7;
    sub_51FB0((__int64)(v7 + 16), v2);
    *((_QWORD *)v7 + 6) = 0LL;
    *((_QWORD *)v7 + 11) = 0LL;
    *((_QWORD *)v7 + 12) = 0LL;
    *((_QWORD *)v7 + 13) = 0LL;
    *((_QWORD *)v7 + 14) = 0LL;
    *((_QWORD *)v7 + 15) = 0LL;
    *(_OWORD *)(v7 + 56) = 0LL;
    *((_QWORD *)v7 + 9) = 0LL;
    *((_QWORD *)v7 + 10) = 7LL;
    *((_WORD *)v7 + 28) = 0;
    v8 = qword_12E9D60 + 1;
    if ( qword_12E9D60 + 1 < 0 )
      v9 = (float)(int)(v8 & 1 | (v8 >> 1)) + (float)(int)(v8 & 1 | (v8 >> 1));
    else
      v9 = (float)(int)v8;
    v10 = qword_12E9D88;
    if ( qword_12E9D88 < 0 )
    {
      v10 = qword_12E9D88 & 1;
      v11 = (float)(int)(v10 | ((unsigned __int64)qword_12E9D88 >> 1))
          + (float)(int)(v10 | ((unsigned __int64)qword_12E9D88 >> 1));
    }
    else
    {
      v11 = (float)(int)qword_12E9D88;
    }
    if ( *(float *)&dword_12E9D50 < (float)(v9 / v11) )
    {
      sub_1A8800();
      v14 = *(_OWORD *)sub_1A8DF0(v12, &v15, v7 + 16, v4);
    }
    v6 = sub_1A88D0(v10, v4, v14, v7);
  }
  
  return v6 + 48;
}






// Now the other subroutine sub_9A5D00 (without callstack):
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
