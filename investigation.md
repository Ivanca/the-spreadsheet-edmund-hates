RDX at sub_7FF6B39C3F70 (prev sub_329950) is: ==> 000001C94F9FF040 00007FF68D139F98 ....ö... L"Ᏸ貘翶"
48 89 5C 24 10 48 89 6C 24 18 48 89 74 24 20 57 41 56 41 57 48 81 EC 90 00 00 00 4D 8B F1 4D 8B F8 48 8B EA 48 8B D9


So a2 is almost certainly the owner/context object, not the event target.

BindButton(
    MenuController *menu,     // RCX
    Entity *owner,            // RDX
    MovieClip *clip,          // R8
    HousePlaceholder *extra   // R9
);

potenciales:
14 times each
00007FF68C9B79F0
00007FF68C9A6F90


sub_7FF6B4012CC0 handles PreUpdate, update, postupdate
0x962d4b 1st loop
0x962de9 2nd loop
0x962e93 3rd loop


sub_7FF6B40264B0 is onclick (0x9764b0)


the 324th movieclip created is CatMenu, e.g. breakif($breakpointcounter == .324), log(MC {a:rax})


################
The MenuPanel is always the third one created (at 0xE289D)

Menupanel
1st at 0000028DF40C0088
2nd at 0000028DF40C0088
3rd at 0000028DF40C0108
4 0000028DF40C0108
5 0000028DF40C0188
6 0000028DF40C0208
7 0000028DF40C0288
8 0000028DF40C0308
9 0000028DF40C0388
10th 0000028DF40C0408

renderer #33 is the catMenu renderer
movieclip # 353 is catMenu root