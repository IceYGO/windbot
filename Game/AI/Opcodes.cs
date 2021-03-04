﻿namespace WindBot.Game.AI
{
    public static class Opcodes
    {
        public const int OPCODE_ADD = 0x40000000,
        OPCODE_SUB = 0x40000001,
        OPCODE_MUL = 0x40000002,
        OPCODE_DIV = 0x40000003,
        OPCODE_AND = 0x40000004,
        OPCODE_OR = 0x40000005,
        OPCODE_NEG = 0x40000006,
        OPCODE_NOT = 0x40000007,
        OPCODE_ISCODE = 0x40000100,
        OPCODE_ISSETCARD = 0x40000101,
        OPCODE_ISTYPE = 0x40000102,
        OPCODE_ISRACE = 0x40000103,
        OPCODE_ISATTRIBUTE = 0x40000104;
    }
}