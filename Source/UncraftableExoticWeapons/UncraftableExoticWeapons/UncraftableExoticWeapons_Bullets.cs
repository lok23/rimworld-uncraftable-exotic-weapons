using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;

namespace UEW_UncraftableExoticWeapons
{
    public class NU43_HumiliatorBullet : Bullet
    {
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            if (hitThing != null && hitThing is Pawn hitPawn)
            {
                float rand = Rand.Value;
                if (rand < 0.5f && hitPawn.AnythingToStrip())
                {
                    Messages.Message("NU43_HumiliatorBullet_SuccessMessage".Translate(
                                     this.launcher.Label, hitPawn.Label
                                    ), MessageTypeDefOf.NeutralEvent);
                    hitPawn.DropAndForbidEverything();
                    hitPawn.apparel.DropAll(hitPawn.Position, true, true);
                    Log.Message(rand.ToString());
                    Log.Message("HUMILIATOR: less than 0.5");
                }
                else
                {
                    Log.Message(rand.ToString());
                    Log.Message("HUMILIATOR: greater than 0.5, or hit a non-strippable enemy");
                }
            }
        }
    }

    public class A34_HarvesterBullet : Bullet
    {
        public ModExtension_A34_HarvesterBullet Props => base.def.GetModExtension<ModExtension_A34_HarvesterBullet>();
        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            if (hitThing != null && hitThing is Pawn hitPawn)
            {
                Log.Message("pawn hit: " + hitPawn.Label);
                Log.Message("BodyPartDef: " + hitPawn.RaceProps.body.GetPartAtIndex(0).def.label);
                Log.Message("BodyPartRecord: " + hitPawn.RaceProps.body.GetPartAtIndex(0).ToString());
                Log.Message("bodydef hit: " + hitPawn.RaceProps.body.label);

                float rand = Rand.Value;
                if (rand < 0.5f && hitPawn.RaceProps.Humanlike)
                {
                    // random organ
                    ThingDef thingDef = (from def in DefDatabase<ThingDef>.AllDefs
                                         where def.IsNaturalOrgan
                                         select def).RandomElementWithFallback(null);

                    // BodyPartRecord part corresponds to the random organ
                    BodyPartRecord part;

                    Log.Message("thingDef label: " + thingDef.label);

                    if (thingDef.label == "heart")
                    {
                        part = hitPawn.RaceProps.body.GetPartAtIndex(6);
                    }
                    else if (thingDef.label == "lung")
                    {
                        part = hitPawn.RaceProps.body.GetPartAtIndex(7);
                        if (hitPawn.health.hediffSet.PartIsMissing(part))
                        {
                            Log.Message("missing left lung, getting right lung");
                            part = hitPawn.RaceProps.body.GetPartAtIndex(8);
                        }
                    }
                    else if (thingDef.label == "kidney")
                    {
                        part = hitPawn.RaceProps.body.GetPartAtIndex(9);
                        if (hitPawn.health.hediffSet.PartIsMissing(part))
                        {
                            Log.Message("missing left kidney, getting right kidney");
                            part = hitPawn.RaceProps.body.GetPartAtIndex(10);
                        }
                    }
                    else // this is liver
                    {
                        part = hitPawn.RaceProps.body.GetPartAtIndex(11);
                    }

                    Log.Message("part currhealth" + hitPawn.health.hediffSet.GetPartHealth(part).ToString());
                    Log.Message("part maxhealth: " + part.def.GetMaxHealth(hitPawn).ToString());

                    // if the organ is undamaged we can harvest it
                    if (hitPawn.health.hediffSet.GetPartHealth(part) == part.def.GetMaxHealth(hitPawn))
                    {
                        GenSpawn.Spawn(thingDef, hitPawn.Position, Find.CurrentMap);

                        Messages.Message("A34_HarvesterBullet_SuccessMessage".Translate(
                                         this.launcher.Label, hitPawn.Label, thingDef.label
                                        ), MessageTypeDefOf.NeutralEvent);
                    }

                    // remove organ
                    Hediff hediff = HediffMaker.MakeHediff(Props.hediffToAdd, hitPawn, part);
                    hitPawn.health.AddHediff(hediff);

                    Log.Message(rand.ToString());
                    Log.Message("HARVESTER: less than 0.5");
                }
                else
                {
                    Log.Message(rand.ToString());
                    Log.Message("HARVESTER: greater than 0.5, or hit a non harvestable enemy");
                }
            }
        }
    }
}