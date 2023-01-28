﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inv.Support;

namespace Pathos
{
  public sealed class CodexApetites : CodexPage<ManifestApetites, ApetiteEditor, Apetite>
  {
    private CodexApetites() { }
#if MASTER_CODEX
    internal CodexApetites(Codex Codex)
      : base(Codex.Manifest.Apetites)
    {
      var Properties = Codex.Properties;
      var Glyphs = Codex.Glyphs;

      Apetite AddStatus(string Name, Inv.Colour Colour, int Threshold, Action<ApetiteEditor> EditorAction)
      {
        return Register.Add(E =>
        {
          E.Name = Name;
          E.Colour = Colour;
          E.Threshold = Threshold;
          E.Glyph = Glyphs.apetite;

          CodexRecruiter.Enrol(() => EditorAction(E));
        });
      }

      satiated = AddStatus("satiated", Inv.Colour.DarkGreen, 1000, A =>
      {
        A.SpeedModifier = -0.25F; // overfed is a slight speed penalty.
        A.PriceMultiplier = 1;
        A.Interrupt = true;
      });

      content = AddStatus("content", Inv.Colour.Black, 200, A =>
      {
        A.PriceMultiplier = 1;
      });

      hungry = AddStatus("hungry", Inv.Colour.DarkYellow, 50, A =>
      {
        A.AbleAtWill = false;
        A.PriceMultiplier = 2;
        A.Interrupt = true;
      });

      weak = AddStatus("weak", Inv.Colour.DarkOrange, 1, A =>
      {
        A.StrengthModifier = -1;
        A.AbleAtWill = false;
        A.PriceMultiplier = 3;
        A.Interrupt = true;
      });

      starving = AddStatus("starving", Inv.Colour.DarkRed, 0, A =>
      {
        A.StrengthModifier = -1;
        A.SpeedModifier = +0.25F; // starving is slight speed bonus.
        A.AbleAtWill = false;
        A.RecoveryRateOverride = Rules.StarvationRecoveryRate;
        A.PriceMultiplier = 4;
        A.Interrupt = true;

        A.SetSymptom(Chance.OneIn45, S =>
        {
          S.ApplyTransient(Properties.fainting, 1.d12() + 8);

          /* TODO: original algorithm had more nuance - you'd faint more often when closer to starvation:
          if (Character.Nutrition.IsStarving && !PropertySet.Contains(FaintingProperty) && Chance.OneIn20.Hit() && !Character.IsInediate())
          {
            var ScaledScore = -(Character.Nutrition.Score / 32);
            var FaintingLevel = 20 + ScaledScore;

            if (1.d(FaintingLevel).Roll() >= 20)
            {
              // minimum 8 turns of fainting.
              ExtendTransient(Extender: null, Character, FaintingProperty, Delay.FromTurns((ScaledScore + 8) * GetActionDelay(Character).GetTurns()));
            }
          }
          */
        });
      });
    }
#endif

    public readonly Apetite satiated;
    public readonly Apetite content;
    public readonly Apetite hungry;
    public readonly Apetite weak;
    public readonly Apetite starving;
  }
}