using System;
using TMPro;
using UnityEngine;

namespace IdleRestaurant
{
    [Serializable]
    public sealed class RestaurantExperienceSettings
    {
        [Range(.35f, .65f)] public float playfieldHeight = .50f;
        [Range(0, .55f)] public float spotlightOpacity = .38f;
        [Range(1, 5)] public float upgradeFeedbackSeconds = 2.2f;
        [Range(2, 6)] public float rewardSeconds = 3.4f;
        public Color focusColor = new Color32(247, 198, 68, 255);
    }

    public sealed partial class RestaurantApp
    {
        [Header("Presentation only - does not change the economy")]
        public RestaurantExperienceSettings experience = new RestaurantExperienceSettings();
        RectTransform experienceLayer, rewardCard, upgradeChip, worldCoach;
        RectTransform[] shades = new RectTransform[4];
        TMP_Text rewardTitle, rewardValue, rewardNext, upgradeMessage, coachMessage, targetPointer;
        readonly Vector3[] spotlightCorners = new Vector3[4];
        static readonly string[] JourneyLabels = { "WELCOME!", "ORDER HERE", "FRESH FOOD COOKING", "MEAL READY", "EAT & ENJOY" };
        static readonly string[] PaymentStages = { "ORDER", "COOKING", "MEAL" };
        CanvasGroup rewardGroup, upgradeGroup;
        readonly TMP_Text[] paymentLabels = new TMP_Text[3];
        readonly float[] paymentAt = { -100, -100, -100 };
        float rewardAt = -100, upgradeAt = -100;
        int feedbackStage, presentationOutlet = -1;
        int coachStep;
        Vector3 coachPosition;
        static readonly string[] UpgradePurpose = {
            "Take orders faster", "A better menu earns more", "Earn more from each order",
            "Guests collect meals faster", "Prepare meals faster", "Earn more from cooking",
            "Make tables ready sooner", "More chances for guest tips", "Earn more from each meal"
        };
        static readonly Vector3[] StationPoints = {
            new Vector3(-2.8f, 1.9f, 4), new Vector3(.4f, 1.9f, 4), new Vector3(0, 1.6f, -1.5f)
        };

        void BuildExperience()
        {
            experienceLayer = Rect("Experience feedback", safe, Vector2.zero, Vector2.one);
            for (int i = 0; i < shades.Length; i++)
            {
                shades[i] = Panel("Tutorial spotlight shade " + i, experienceLayer, Vector2.zero, Vector2.zero, Color.clear);
                shades[i].GetComponent<UnityEngine.UI.Image>().raycastTarget = false;
            }
            targetPointer = Text(experienceLayer, "▼", Vector2.zero, Vector2.zero, 30, GuideYellow, TextAlignmentOptions.Center);
            targetPointer.rectTransform.sizeDelta = new Vector2(46, 40);
            worldCoach = Panel("Guest journey callout", experienceLayer, Vector2.zero, Vector2.zero, Green);
            worldCoach.sizeDelta = new Vector2(240, 64);
            coachMessage = Text(worldCoach, "", new Vector2(.03f, .06f), new Vector2(.97f, .94f), 22, Cream, TextAlignmentOptions.Center);
            coachMessage.enableAutoSizing = true; coachMessage.fontSizeMin = 18;
            var edge = worldCoach.gameObject.AddComponent<UnityEngine.UI.Outline>(); edge.effectColor = GuideYellow; edge.effectDistance = new Vector2(2, 2);
            upgradeChip = Panel("Upgrade result", experienceLayer, new Vector2(.15f, .735f), new Vector2(.85f, .785f), PriceGreen);
            upgradeGroup = upgradeChip.gameObject.AddComponent<CanvasGroup>(); upgradeGroup.blocksRaycasts = false;
            upgradeMessage = Text(upgradeChip, "", new Vector2(.03f, .05f), new Vector2(.97f, .95f), 23, Cream, TextAlignmentOptions.Center);
            upgradeMessage.enableAutoSizing = true; upgradeMessage.fontSizeMin = 18;
            for (int i = 0; i < paymentLabels.Length; i++)
            {
                paymentLabels[i] = Text(experienceLayer, "", Vector2.zero, Vector2.zero, 21, PriceGreen, TextAlignmentOptions.Center);
                paymentLabels[i].fontStyle = FontStyles.Bold;
                paymentLabels[i].rectTransform.sizeDelta = new Vector2(230, 40);
            }
            rewardCard = Panel("Service reward", experienceLayer, new Vector2(.10f, .56f), new Vector2(.90f, .73f), Cream);
            rewardGroup = rewardCard.gameObject.AddComponent<CanvasGroup>(); rewardGroup.blocksRaycasts = false;
            var border = rewardCard.gameObject.AddComponent<UnityEngine.UI.Outline>(); border.effectColor = GuideYellow; border.effectDistance = new Vector2(3, 3);
            rewardTitle = Text(rewardCard, "SERVICE COMPLETE!", new Vector2(.04f, .70f), new Vector2(.96f, .96f), 25, Green, TextAlignmentOptions.Center); rewardTitle.fontStyle = FontStyles.Bold;
            rewardValue = Text(rewardCard, "", new Vector2(.04f, .34f), new Vector2(.96f, .70f), 32, PriceGreen, TextAlignmentOptions.Center); rewardValue.fontStyle = FontStyles.Bold;
            rewardNext = Text(rewardCard, "", new Vector2(.05f, .05f), new Vector2(.95f, .33f), 20, Ink, TextAlignmentOptions.Center);
            // All presentation elements are non-blocking; only the existing guide Skip is interactive.
            foreach (var graphic in experienceLayer.GetComponentsInChildren<UnityEngine.UI.Graphic>()) graphic.raycastTarget = false;
            guidePanel.SetAsLastSibling();
        }

        int GuestJourney()
        {
            coachPosition = new Vector3(-4.15f, 1.4f, -3.8f);
            var all = Game.State.outlets[Game.State.selected].service;
            if (all == null || all.Length == 0) return 0;
            var g = all[0]; coachPosition = new Vector3(g.x, 1.6f, g.z);
            if (g.phase <= GuestPhase.ToOrder) return 0;
            if (g.phase == GuestPhase.Ordering) return 1;
            if (g.phase <= GuestPhase.WaitingFood) { coachPosition = StationPoints[1]; return 2; }
            if (g.phase <= GuestPhase.ToSeat) return 3;
            return 4;
        }

        string JourneyInstruction(int step)
        {
            switch (step)
            {
                case 0: return "1 / 5   Our first guest is coming to order.";
                case 1: return "1 / 5   Your host takes the order and earns coins.";
                case 2: return "1 / 5   The kitchen prepares a fresh meal.";
                case 3: return "1 / 5   Your guest collects food and finds a table.";
                default: return "1 / 5   A happy guest finishes their meal and pays.";
            }
        }

        void UpgradeResult(int card)
        {
            feedbackStage = card / 3; upgradeAt = Time.unscaledTime;
            upgradeMessage.text = UpgradePurpose[card];
            // This feedback identifies the affected station; it does not exaggerate a small speed increase.
        }

        void PaymentResult(int step, double amount)
        {
            if (!experienceLayer) return;
            if (step == 3)
            {
                rewardAt = Time.unscaledTime;
                int meals = Game.State.outlets[Game.State.selected].seated;
                rewardTitle.text = meals + " HAPPY GUESTS · SERVICE COMPLETE!";
                rewardValue.text = "+" + Economy.Money(amount) + "    +1 TICKET";
                rewardNext.text = !Game.State.introComplete ? "Next: open the map and grow your restaurant" : "Your next service starts automatically";
                return;
            }
            if (step < 0 || step > 2 || Time.unscaledTime - paymentAt[step] < .65f) return;
            paymentAt[step] = Time.unscaledTime;
            paymentLabels[step].text = Economy.Money(amount);
        }

        void PositionAtWorld(RectTransform target, Vector3 position, float offset)
        {
            if (!gameCamera || !world.room) return;
            Vector3 point = gameCamera.WorldToScreenPoint(world.room.TransformPoint(position));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(safe, point, null, out var local);
            float half = target.sizeDelta.x / 2 + 8;
            local.x = Mathf.Clamp(local.x, safe.rect.xMin + half, safe.rect.xMax - half);
            target.anchorMin = target.anchorMax = new Vector2(.5f, .5f);
            target.anchoredPosition = local + Vector2.up * offset;
        }

        void UpdateExperience()
        {
            if (!experienceLayer || Game == null) return;
            bool play = page == "play" && !modal;
            if (presentationOutlet != Game.State.selected)
            {
                presentationOutlet = Game.State.selected; rewardAt = upgradeAt = -100;
                for (int i = 0; i < 3; i++) paymentAt[i] = -100;
            }
            float rewardElapsed = Time.unscaledTime - rewardAt;
            rewardGroup.alpha = play ? Mathf.Clamp01((experience.rewardSeconds - rewardElapsed) * 3) : 0;
            float upElapsed = Time.unscaledTime - upgradeAt;
            upgradeGroup.alpha = play ? Mathf.Clamp01((experience.upgradeFeedbackSeconds - upElapsed) * 3) : 0;
            // Fixed-size bounded animation; reduced motion retains the same readable feedback without movement.
            float scale = Game.State.reducedMotion ? 1 : 1 + .045f * Mathf.Sin(Mathf.Clamp01(rewardElapsed / .35f) * Mathf.PI);
            rewardCard.localScale = Vector3.one * scale;
            for (int i = 0; i < 3; i++)
            {
                float age = Time.unscaledTime - paymentAt[i];
                paymentLabels[i].gameObject.SetActive(play && age < 1.25f && rewardGroup.alpha == 0);
                if (age < 1.25f) { PositionAtWorld(paymentLabels[i].rectTransform, StationPoints[i], 12 + (Game.State.reducedMotion ? 0 : age * 22)); var c = PriceGreen; c.a = Mathf.Clamp01((1.25f - age) * 3); paymentLabels[i].color = c; }
            }
            coachStep = GuestJourney();
            bool first = play && Game.Guide == 1 && !Game.State.introComplete && guideGroup.alpha > .9f;
            worldCoach.gameObject.SetActive(first);
            if (first)
            {
                coachMessage.text = JourneyLabels[coachStep];
                PositionAtWorld(worldCoach, coachPosition, 42);
            }
            RectTransform target = null;
            if (play && guideWanted && guideGroup.alpha > .9f)
            {
                if (Game.Guide == 2 && cards[0]) target = (RectTransform)cards[0].transform;
                else if (Game.Guide == 3) target = stage == 1 ? (RectTransform)cards[0].transform : (RectTransform)stageButtons[1].transform;
                else if (Game.Guide == 5) target = (RectTransform)navButtons[0].transform;
            }
            UpdateSpotlight(target);
            if (play && upElapsed < experience.upgradeFeedbackSeconds && !target && !first && rewardGroup.alpha == 0)
            {
                targetPointer.gameObject.SetActive(true);
                PositionAtWorld(targetPointer.rectTransform, StationPoints[feedbackStage], 8);
            }
        }

        void UpdateSpotlight(RectTransform target)
        {
            targetPointer.gameObject.SetActive(target);
            foreach (var shade in shades) shade.gameObject.SetActive(target);
            if (!target) return;
            var corners = spotlightCorners; target.GetWorldCorners(corners);
            Vector2 min = safe.InverseTransformPoint(corners[0]), max = safe.InverseTransformPoint(corners[2]);
            min = (min - safe.rect.min - Vector2.one * 6) / safe.rect.size;
            max = (max - safe.rect.min + Vector2.one * 6) / safe.rect.size;
            min = Vector2.Max(Vector2.zero, min); max = Vector2.Min(Vector2.one, max);
            SetShade(0, Vector2.zero, new Vector2(1, min.y));
            SetShade(1, new Vector2(0, max.y), Vector2.one);
            SetShade(2, new Vector2(0, min.y), new Vector2(min.x, max.y));
            SetShade(3, new Vector2(max.x, min.y), new Vector2(1, max.y));
            targetPointer.rectTransform.anchorMin = targetPointer.rectTransform.anchorMax = new Vector2((min.x + max.x) * .5f, max.y);
            targetPointer.rectTransform.anchoredPosition = new Vector2(0, 25);
        }

        void SetShade(int index, Vector2 min, Vector2 max)
        {
            shades[index].anchorMin = min; shades[index].anchorMax = max;
            shades[index].offsetMin = shades[index].offsetMax = Vector2.zero;
            shades[index].GetComponent<UnityEngine.UI.Image>().color = new Color(.055f, .10f, .075f, experience.spotlightOpacity * guideGroup.alpha);
        }

        void FrameRestaurant()
        {
            if (!gameCamera || page != "play") return;
            var area = Screen.safeArea;
            gameCamera.rect = new Rect(area.x / Screen.width, (area.y + area.height * .315f) / Screen.height,
                area.width / Screen.width, area.height * experience.playfieldHeight / Screen.height);
            // Fit the complete route and back scenery at each device aspect ratio, without camera drift.
            gameCamera.transform.position = new Vector3(0, 16, -19);
            gameCamera.transform.LookAt(new Vector3(0, .6f, .4f));
            float width = 0, height = 0;
            for (int corner = 0; corner < 8; corner++)
            {
                Vector3 p = gameCamera.transform.InverseTransformPoint(new Vector3((corner & 1) == 0 ? -5.7f : 5.7f, (corner & 2) == 0 ? -.2f : 3.6f, (corner & 4) == 0 ? -6.2f : 6.7f));
                width = Mathf.Max(width, Mathf.Abs(p.x)); height = Mathf.Max(height, Mathf.Abs(p.y));
            }
            gameCamera.orthographicSize = Mathf.Max(height, width / gameCamera.aspect) * 1.025f;
        }
    }
}
