﻿using System;

namespace BinarySerializer.Klonoa
{
    public abstract class LoaderConfiguration_LV : LoaderConfiguration
    {
        public virtual int LanguagesCount => 1;
        public bool HasMultipleLanguages => LanguagesCount > 1;

        public virtual string FilePath_KL => "BGMPACK.BIN";

        public virtual string GetFilePath(Loader_LV.BINType bin, int languageIndex = 0)
        {
            return bin switch
            {
                Loader_LV.BINType.KL => HasMultipleLanguages ? $"KLDATA{languageIndex}.BIN" : "KLDATA.BIN",
                Loader_LV.BINType.BGM => "BGMPACK.BIN",
                Loader_LV.BINType.PPT => "PPTPACK.BIN",
                _ => throw new ArgumentOutOfRangeException(nameof(bin), bin, null)
            };
        }
    }
}