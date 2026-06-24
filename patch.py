"""
patch_swf_exports.py
Replaces SymbolClass (tag 76, AS3) with DefineExportAssets (tag 56, AS2)
so the game's SWF parser recognises _Append_* symbols.
Also removes DoABC (tag 82) which is unused AS3 bytecode.
"""
import struct, zlib
from pathlib import Path

INPUT  = r"D:\Games\mewgenics\mods\UIModTest\swfs\house_table_stats.swf"
OUTPUT = r"D:\Games\mewgenics\mods\UIModTest\swfs\house_table_stats.swf"  # overwrite in-place

def patch(input_path, output_path):
    raw = Path(input_path).read_bytes()
    sig, ver = raw[:3], raw[3]

    if sig == b'CWS':
        body = bytearray(raw[:8]) + bytearray(zlib.decompress(raw[8:]))
    elif sig == b'FWS':
        body = bytearray(raw)
    else:
        raise ValueError(f"Unsupported: {sig}")

    # find where tags start (skip header + RECT + FrameRate + FrameCount)
    pos = 8
    nbits = (body[pos] >> 3) & 0x1F
    pos += (5 + 4 * nbits + 7) // 8 + 4   # RECT + FrameRate(2) + FrameCount(2)
    header_end = pos

    # --- first pass: collect SymbolClass entries ---
    export_entries = []
    p = header_end
    while p + 2 <= len(body):
        h = struct.unpack_from('<H', body, p)[0]
        ttype = h >> 6; tlen = h & 0x3F; p += 2
        extra = 0
        if tlen == 0x3F:
            tlen = struct.unpack_from('<i', body, p)[0]; p += 4; extra = 4
        if ttype == 76:
            count = struct.unpack_from('<H', body, p)[0]; q = p + 2
            for _ in range(count):
                cid = struct.unpack_from('<H', body, q)[0]; q += 2
                end = body.index(0, q); name = bytes(body[q:end]).decode(); q = end + 1
                export_entries.append((cid, name))
        if ttype == 0: break
        p += tlen

    print(f"SymbolClass entries captured: {export_entries}")

    # --- second pass: rebuild tag stream ---
    new_tags = bytearray()

    def emit_tag(ttype, data: bytes):
        if len(data) < 0x3F:
            new_tags.extend(struct.pack('<H', (ttype << 6) | len(data)) + data)
        else:
            new_tags.extend(struct.pack('<H', (ttype << 6) | 0x3F) + struct.pack('<i', len(data)) + data)

    p = header_end
    while p + 2 <= len(body):
        h = struct.unpack_from('<H', body, p)[0]
        ttype = h >> 6; tlen = h & 0x3F; p += 2
        if tlen == 0x3F:
            tlen = struct.unpack_from('<i', body, p)[0]; p += 4
        tag_data = bytes(body[p:p+tlen]); p += tlen

        if ttype == 76:   # SymbolClass → skip (replaced by tag 56)
            print(f"  removed Tag 76 (SymbolClass)")
            continue
        if ttype == 82:   # DoABC → skip (unused AS3 bytecode)
            print(f"  removed Tag 82 (DoABC, {tlen} bytes)")
            continue
        if ttype == 69:   # FileAttributes → clear AS3 flag (bit 3)
            fa = bytearray(tag_data)
            fa[0] = fa[0] & ~0x08   # clear ActionScript3 bit
            tag_data = bytes(fa)
            print(f"  FileAttributes: cleared AS3 bit → {fa[0]:#04x}")
        if ttype == 0:    # End → inject tag 56 first
            if export_entries:
                t56 = struct.pack('<H', len(export_entries))
                for cid, name in export_entries:
                    t56 += struct.pack('<H', cid) + name.encode() + b'\x00'
                emit_tag(56, t56)
                print(f"  inserted Tag 56 (DefineExportAssets) with {len(export_entries)} entry/entries")
            emit_tag(0, b'')
            break

        emit_tag(ttype, tag_data)

    # --- reassemble ---
    new_body = bytes(body[:header_end]) + bytes(new_tags)
    uncompressed_len = len(new_body)
    compressed = zlib.compress(new_body[8:], 9)
    out = b'CWS' + bytes([ver]) + struct.pack('<I', uncompressed_len) + compressed

    Path(output_path).write_bytes(out)
    print(f"\nPatched SWF written to: {output_path}")
    print(f"  original: {len(raw)} bytes  →  patched: {len(out)} bytes")

    # verify
    print("\n--- verification ---")
    body2 = bytearray(out[:8]) + bytearray(zlib.decompress(out[8:]))
    p = header_end
    while p + 2 <= len(body2):
        h = struct.unpack_from('<H', body2, p)[0]
        ttype = h >> 6; tlen = h & 0x3F; p += 2
        if tlen == 0x3F: tlen = struct.unpack_from('<i', body2, p)[0]; p += 4
        if ttype == 56:
            count = struct.unpack_from('<H', body2, p)[0]; q = p + 2
            print(f"[Tag 56] DefineExportAssets — {count} entries:")
            for i in range(count):
                cid = struct.unpack_from('<H', body2, q)[0]; q += 2
                end = body2.index(0, q); name = bytes(body2[q:end]).decode(); q = end + 1
                print(f"  charID={cid}  name='{name}'")
        if ttype == 76: print("[Tag 76] SymbolClass STILL PRESENT — ERROR")
        if ttype == 82: print("[Tag 82] DoABC STILL PRESENT")
        if ttype == 0: break
        p += tlen

patch(INPUT, OUTPUT)