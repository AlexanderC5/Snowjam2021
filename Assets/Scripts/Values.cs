using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Values : MonoBehaviour
{
    // Backgrounds (B)
    // public enum BGs { TITLE, KITCHEN, COUNTERTOP, ROOM, CELEBRATION };
    public const int B_TITLE = 0;
    public const int B_KITCHEN = 1;
    public const int B_COUNTERTOP = 2;
    public const int B_ROOM = 3;
    public const int B_CELEBRATION = 4;

    // Characters (C)
    // public enum CHARs { STUART, TOA };
    public const int C_STUART = 0;
    public const int C_TOA = 1;

    // UI (U)
    // public enum UIs { DIALOGUE, NAMEPLATE, START, OPTIONS, EXIT, OPT_MENU, SETTINGS, SCENE_TITLE };
    public const int U_DIALOGUE = 0;
    public const int U_NAMEPLATE = 1;
    public const int U_START = 2;
    public const int U_OPTIONS = 3;
    public const int U_EXIT = 4;
    public const int U_OPT_MENU = 5;
    public const int U_SETTINGS = 6;
    public const int U_SCENE_TITLE = 7;

    // Ingredients (I)
    //public enum INGRs
    //{
    //    CUT_BOARD, SINK, POT, POT_WATER, MILK, BUTTER, SALT, PEPPER, BOWL, KNIFE,
    //    POTATO, POTATO_PEELED, BUTTER_SLICE, PEELER, POTATO_STEAM, POTATO_BOWL, MIXER, POTATO_MASH, POTATO_GRAVY, GRAVY,
    //    BOWL_FILLED, SUGAR, SPICES, FLOUR, EGG, ICING, GINGERBREAD, GINGER_ICED
    //};
    // CUT_BOARD and SINK are special ingredients - they're stationary, and don't have box colliders, but rather 
    //  arrays that store their bounds on the scene. This was done since adding colliders to these objects would
    //  sometimes block the other ingredients from being dragged.
    public const int I_CUT_BOARD = 0;
    public const int I_SINK = 1;
    public const int I_POT = 2;
    public const int I_POT_WATER = 3;
    public const int I_MILK = 4;
    public const int I_BUTTER = 5;
    public const int I_SALT = 6;
    public const int I_PEPPER = 7;
    public const int I_BOWL = 8;
    public const int I_KNIFE = 9;
    public const int I_POTATO = 10;
    public const int I_POTATO_PEELED = 11;
    public const int I_BUTTER_SLICE = 12;
    public const int I_PEELER = 13;
    public const int I_POTATO_STEAM = 14;
    public const int I_POTATO_BOWL = 15;
    public const int I_MIXER = 16;
    public const int I_POTATO_MASH = 17;
    public const int I_POTATO_GRAVY = 18;
    public const int I_GRAVY = 19;
    public const int I_BOWL_FILLED = 20;
    public const int I_SUGAR = 21;
    public const int I_SPICES = 22;
    public const int I_FLOUR = 23;
    public const int I_EGG = 24;
    public const int I_ICING = 25;
    public const int I_GINGERBREAD = 26;
    public const int I_GINGER_ICED = 27;
    public const int I_HAM = 28;
    public const int I_CLOVES = 29;
    public const int I_PINEAPPLE = 30;
    public const int I_PINE_HAM = 31;
    public const int I_PINE_HAM_CLOVE = 32;
    public const int I_BREAD_LOAF = 33;
    public const int I_BREAD_A = 34;
    public const int I_MAYO = 35;
    public const int I_SPOON = 36;
    public const int I_SPOON_MAYO = 37;
    public const int I_BREAD_B = 38;
    public const int I_TOMATO = 39;
    public const int I_TOMATO_SLICE = 40;
    public const int I_BREAD_C = 41;
    public const int I_LETTUCE = 42;
    public const int I_LETTUCE_LEAF = 43;
    public const int I_BREAD_D = 44;
    public const int I_CHEESE_SLICE = 45;
    public const int I_BREAD_E = 46;
    public const int I_HAM_SLICE = 47;
    public const int I_BREAD_F = 48;
    public const int I_SANDWICH = 49;

    // SFX (S)
    public const int S_POP = 0;
    public const int S_TEXT_A = 1;
    public const int S_TEXT_B = 2;
    public const int S_FALLING = 3;
    public const int S_COOKING_COMPLETE = 4;
    public const int S_SINK_RUNNING = 5;
    public const int S_OVEN_DING = 6;
    public const int S_PEELING = 7;
    public const int S_KNIFE_CHOP = 8;
    public const int S_POTATO_MASH = 9;

    // Expressions (E)
    public const int E_Conc = 0;
    public const int E_ConcBlush = 1;
    public const int E_ConcBlushTalk = 2;
    public const int E_ConcTalk = 3;
    public const int E_EyeC = 4;
    public const int E_EyeCBlush = 5;
    public const int E_EyeCBlushTalk = 6;
    public const int E_EyeCTalk = 7;
    public const int E_Shock = 8;
    public const int E_ShockBlush = 9;
    public const int E_ShockBlushTalk = 10;
    public const int E_ShockTalk = 11;
    public const int E_Smile = 12;
    public const int E_SmileBlush = 13;
    public const int E_SmileBlushTalk = 14;
    public const int E_SmileTalk = 15;
    public const int E_Support = 16;
    public const int E_SupportBlush = 17;
    public const int E_SupportBlushTalk = 18;
    public const int E_SupportTalk = 19;
}
