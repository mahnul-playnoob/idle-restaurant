using System;
using TMPro;
using UnityEngine;

namespace IdleRestaurant
{
    // The restaurant's UI palette: cream paper, dark herb ink, herb-green actions, mustard guidance.
    public sealed partial class RestaurantApp
    {
        static Color UpgradeGreen => Hex("#287548");
        static Color PriceGreen => Hex("#1C5535");
        static Color Unavailable => Hex("#E0E1DB");
        static Color UnavailablePrice => Hex("#C3C7BF");
        Color GuideYellow => experience.focusColor;

        readonly UnityEngine.UI.Button[] navButtons = new UnityEngine.UI.Button[5];
        readonly UnityEngine.UI.Image[] priceBars = new UnityEngine.UI.Image[3];
        readonly TMP_Text[] priceLabels = new TMP_Text[3];
        CanvasGroup guideGroup;
        string guidePendingText;
        int guideKey = int.MinValue;
        float guideChangedAt;
        bool guideWanted, guideTextSwapped;
        static readonly string[] NavigationPages = { "map", "managers", "play", "research", "tasks" };

        void BuildUpgradeCard(RectTransform parent, int slot)
        {
            int card = stage * 3 + slot;
            var button = Button(parent, "", new Vector2(slot / 3f + .005f, 0),
                new Vector2((slot + 1) / 3f - .005f, 1), () => Buy(card), UpgradeGreen);
            button.name = "Upgrade " + card;
            cards[slot] = button;
            cardImages[slot] = button.GetComponent<UnityEngine.UI.Image>();
            var title = button.GetComponentInChildren<TMP_Text>();
            cardLabels[slot] = title;
            title.rectTransform.anchorMin = new Vector2(.015f, .32f);
            title.rectTransform.anchorMax = new Vector2(.985f, .985f);
            title.fontSize = Game.State.largeText ? 25 : 23;
            title.enableAutoSizing = true;
            title.fontSizeMin = Game.State.largeText ? 21 : 19;
            title.fontSizeMax = title.fontSize;
            title.alignment = TextAlignmentOptions.Center;
            var strip = Panel("Upgrade price bar", button.transform,
                new Vector2(.035f, .04f), new Vector2(.965f, .29f), PriceGreen);
            priceBars[slot] = strip.GetComponent<UnityEngine.UI.Image>();
            priceBars[slot].raycastTarget = false;
            priceLabels[slot] = Text(strip, "", Vector2.zero, Vector2.one, 25, Cream, TextAlignmentOptions.Center);
            priceLabels[slot].fontStyle = FontStyles.Bold;
            priceLabels[slot].enableAutoSizing = true;
            priceLabels[slot].fontSizeMin = 18;
            priceLabels[slot].fontSizeMax = Game.State.largeText ? 28 : 25;
            priceLabels[slot].textWrappingMode = TextWrappingModes.NoWrap;
        }

        void RefreshUpgradeCard(int slot)
        {
            if (!cards[slot] || !cardLabels[slot]) return;
            int card = stage * 3 + slot, level = Game.State.outlets[Game.State.selected].upgrades[card];
            bool max = level >= catalog.maxUpgrade;
            double price = Game.Cost(Game.State.selected, card);
            bool guided = Game.State.introComplete || Game.Guide >= 4 ||
                (Game.Guide == 2 && card == 0) || (Game.Guide == 3 && card == 3);
            bool affordable = !max && guided && Game.State.cash >= price;
            cardImages[slot].color = affordable ? UpgradeGreen : Unavailable;
            cardLabels[slot].color = affordable ? Cream : Ink;
            priceBars[slot].color = affordable ? PriceGreen : UnavailablePrice;
            priceLabels[slot].color = affordable ? Cream : Ink;
            string effect = Game.Family ? FamilyEffect(card, level) : EffectText(card, level);
            string next = max ? "" : " → " + (Game.Family ? FamilyEffect(card, level + 1) : EffectText(card, level + 1));
            cardLabels[slot].text = UpgradeNames[card] + "\nLv. " + level + " / " + catalog.maxUpgrade + "\n" + effect + next;
            priceLabels[slot].text = max ? "MAX LEVEL" : !guided ? "SOON" :
                affordable ? "BUY  " + Economy.Money(price) : Economy.Money(price);
            cards[slot].interactable = affordable;
        }

        void RefreshNavigation()
        {
            for (int i = 0; i < navButtons.Length; i++)
            {
                if (!navButtons[i]) continue;
                bool selected = page == NavigationPages[i] || (i == 4 && page == "rewards");
                navButtons[i].GetComponent<UnityEngine.UI.Image>().color = selected ? Green : Hex("#E8E6DA");
                navButtons[i].GetComponentInChildren<TMP_Text>().color = selected ? Cream : Ink;
            }
        }

        void SetupGuidePresentation()
        {
            guideGroup = guidePanel.gameObject.AddComponent<CanvasGroup>();
            guideGroup.alpha = 0;
            guideGroup.blocksRaycasts = false;
            var edge = guidePanel.gameObject.AddComponent<UnityEngine.UI.Outline>();
            edge.effectColor = GuideYellow;
            edge.effectDistance = new Vector2(2, 2);
            guideText.fontStyle = FontStyles.Bold;
            guideText.enableAutoSizing = true;
            guideText.fontSizeMin = 20;
            guideText.fontSizeMax = 25;
        }

        void RefreshGuidePresentation()
        {
            if (!guidePanel || Game == null) return;
            guideWanted = page == "play" && !Game.State.introComplete && Game.Guide > 0;
            // Kitchen selection is a distinct visual instruction even though it belongs to model step 3.
            int key = guideWanted ? Game.Guide * 10 + (Game.Guide == 1 ? GuestJourney() : Game.Guide == 3 && stage == 1 ? 1 : 0) : -1;
            if (key != guideKey)
            {
                guideKey = key;
                guideChangedAt = Time.unscaledTime;
                guideTextSwapped = false;
                switch (Game.Guide)
                {
                    case 1: guidePendingText = JourneyInstruction(GuestJourney()); break;
                    case 2: guidePendingText = "2 / 5   Serve guests faster. Tap Faster orders."; break;
                    case 3: guidePendingText = stage == 1 ? "3 / 5   Help guests collect meals faster. Tap Faster delivery." : "3 / 5   Tap KITCHEN to improve food delivery."; break;
                    case 4: guidePendingText = "4 / 5   More seats are ready. Finish this service."; break;
                    default: guidePendingText = "5 / 5   Service complete! Tap MAP to grow."; break;
                }
            }
            for (int i = 0; i < stageButtons.Length; i++)
                if (stageButtons[i]) stageButtons[i].interactable = Game.State.introComplete || i == 0 || Game.Guide >= i + 2;
        }

        void AnimateGuide()
        {
            if (!guideGroup || Game == null) return;
            float elapsed = Time.unscaledTime - guideChangedAt;
            bool reduced = Game.State.reducedMotion;
            // Finish the previous prompt, clear it, then introduce the next prompt consistently.
            float fadeOut = reduced ? 0 : .16f;
            float revealAt = reduced ? .18f : .40f;
            if (elapsed < fadeOut) guideGroup.alpha = 1 - elapsed / fadeOut;
            else if (elapsed < revealAt) guideGroup.alpha = 0;
            else
            {
                if (!guideTextSwapped) { guideText.text = guidePendingText; guideTextSwapped = true; }
                guideGroup.alpha = guideWanted ? (reduced ? 1 : Mathf.Clamp01((elapsed - revealAt) / .20f)) : 0;
            }
            guideGroup.blocksRaycasts = guideWanted && guideGroup.alpha > .95f;
            bool highlight = guideWanted && elapsed >= revealAt;
            for (int i = 0; i < cards.Length; i++)
                Glow(cards[i] ? cards[i].gameObject : null, highlight &&
                    ((Game.Guide == 2 && stage == 0 && i == 0) || (Game.Guide == 3 && stage == 1 && i == 0)));
            for (int i = 0; i < stageButtons.Length; i++)
                Glow(stageButtons[i] ? stageButtons[i].gameObject : null, highlight && Game.Guide == 3 && stage != 1 && i == 1);
            for (int i = 0; i < navButtons.Length; i++)
                Glow(navButtons[i] ? navButtons[i].gameObject : null, highlight && Game.Guide == 5 && i == 0);
            Glow(service ? service.gameObject : null, highlight && Game.Guide == 4);
        }

        void Glow(GameObject target, bool active)
        {
            if (!target) return;
            var glow = target.GetComponent<UnityEngine.UI.Outline>();
            if (!glow && active) { glow = target.AddComponent<UnityEngine.UI.Outline>(); glow.effectDistance = new Vector2(4, 4); }
            if (!glow) return;
            glow.enabled = active;
            if (active)
            {
                Color color = GuideYellow;
                color.a = Game.State.reducedMotion ? 1 : .82f + .18f * Mathf.Sin(Time.unscaledTime * 4);
                glow.effectColor = color;
            }
        }
    }
}
