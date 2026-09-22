using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public partial class TheSpredsheetEdmundHates
{
        public static unsafe Button* FindButtonInPanel(
        MenuPanel* menuPanel,
        string name)
    {
        if (menuPanel == null)
            return null;

        ButtonMapNode* header = menuPanel->ButtonMap;

        if (header == null)
            return null;

        // Native:
        //
        // mov r14, [r13+40h]
        // mov rbx, [r14+8]
        //
        ButtonMapNode* node = header->Parent;

        while (node != header)
        {
            string nodeName = HexToAscii((nint)node->Key);

            LogStr(
                $"FindButtonInPanel: node 0x{(nint)node:X} " +
                $"'{nodeName}'");

            int cmp = string.CompareOrdinal(nodeName, name);

            if (cmp == 0)
            {
                LogStr(
                    $"FindButtonInPanel: FOUND '{name}' " +
                    $"-> 0x{(nint)node->Value:X}");

                return node->Value;
            }

            if (cmp < 0)
            {
                LogStr(
                    $"FindButtonInPanel: '{nodeName}' < '{name}' " +
                    $"-> RIGHT 0x{(nint)node->Right:X}");

                node = node->Right;
            }
            else
            {
                LogStr(
                    $"FindButtonInPanel: '{nodeName}' > '{name}' " +
                    $"-> LEFT 0x{(nint)node->Left:X}");

                node = node->Left;
            }
        }
        return null;
    }
}