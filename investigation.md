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