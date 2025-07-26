using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using MoreBadges;
using TMPro;
using BepInEx.Configuration;

namespace IsoBadges
{
    public enum AchievementUIStyle
    {
        Default,
        Compact
    }

    public class AchievementPopupUGUI : MonoBehaviour
    {
        public TextMeshProUGUI achievementTitle;
        public TextMeshProUGUI achievementDescription;
        public Image achievementIcon;
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;

        private MoreBadgesPlugin.CustomBadge _badge;
        private Coroutine _activeAnimation;

        public void Show(MoreBadgesPlugin.CustomBadge badge, int currentProgress)
        {
            this._badge = badge;
            UpdateText(currentProgress);

            if (badge.icon != null)
            {
                achievementIcon.sprite = Sprite.Create(badge.icon, new Rect(0, 0, badge.icon.width, badge.icon.height), new Vector2(0.5f, 0.5f));
            }

            StartAnimation();
        }

        public void UpdateProgress(int newProgress)
        {
            UpdateText(newProgress);
            StartAnimation();
        }

        private void UpdateText(int newProgress)
        {
            bool isComplete = newProgress >= _badge.progressRequired;

            // Handle text based on the UI style from the config
            if (ConfigManager.UIStyle.Value == AchievementUIStyle.Compact)
            {
                if (achievementDescription != null)
                {
                    if (isComplete)
                    {
                        achievementDescription.text = "Unlocked";
                        achievementDescription.color = new Color(1f, 0.84f, 0f); // Gold
                    }
                    else
                    {
                        achievementDescription.text = $"{newProgress}/{_badge.progressRequired}";
                        achievementDescription.color = Color.white;
                    }
                }
            }
            else // Default Style
            {
                if (achievementTitle != null)
                {
                    achievementTitle.text = _badge.displayName;
                    achievementTitle.color = Color.white;
                }

                if (achievementDescription != null)
                {
                    if (isComplete)
                    {
                        achievementDescription.text = "Achievement Unlocked";
                        achievementDescription.color = new Color(1f, 0.84f, 0f); // Gold
                    }
                    else
                    {
                        achievementDescription.text = $"{_badge.description} ({newProgress}/{_badge.progressRequired})";
                        achievementDescription.color = Color.gray;
                    }
                }
            }
        }

        private void StartAnimation()
        {
            if (_activeAnimation != null) StopCoroutine(_activeAnimation);
            _activeAnimation = StartCoroutine(AnimatePopup());
        }

        public bool IsForBadge(string badgeName)
        {
            return _badge != null && _badge.name == badgeName;
        }

        public void CloseImmediate()
        {
            if (_activeAnimation != null) StopCoroutine(_activeAnimation);
            AchievementUIFactory.ClearActivePopup();
            if (rectTransform != null && rectTransform.root != null) Destroy(rectTransform.root.gameObject);
        }

        private IEnumerator AnimatePopup()
        {
            canvasGroup.alpha = 0f;
            float animTime = 0.5f;
            float yStart = 50f;
            float yEnd = -15f;
            float elapsed = 0f;

            while (elapsed < animTime)
            {
                if (rectTransform == null) yield break;
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, Mathf.Lerp(yStart, yEnd, elapsed / animTime));
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / animTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (rectTransform == null) yield break;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, yEnd);
            canvasGroup.alpha = 1f;

            yield return new WaitForSeconds(3.0f);

            elapsed = 0f;
            while (elapsed < animTime)
            {
                if (rectTransform == null) yield break;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / animTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            AchievementUIFactory.ClearActivePopup();
            if (rectTransform != null && rectTransform.root != null) Destroy(rectTransform.root.gameObject);
        }
    }

    public static class AchievementUIFactory
    {
        private static TMP_FontAsset _gameFontAsset;
        private static SFX_Instance _achievementSound;
        private static AchievementPopupUGUI _activePopup;

        private static TMP_FontAsset GameFont
        {
            get
            {
                if (_gameFontAsset == null)
                {
                    _gameFontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
                        .FirstOrDefault(fontAsset => fontAsset.faceInfo.familyName == "Daruma Drop One");
                    if (_gameFontAsset == null) IsoBadgesPlugin.Log.LogWarning("Could not find game font 'Daruma Drop One'.");
                }
                return _gameFontAsset;
            }
        }

        private static SFX_Instance AchievementSound
        {
            get
            {
                if (_achievementSound == null)
                {
                    Bonkable bonkableInstance = Resources.FindObjectsOfTypeAll<Bonkable>().FirstOrDefault();
                    if (bonkableInstance != null && bonkableInstance.bonk != null && bonkableInstance.bonk.Length > 0)
                    {
                        _achievementSound = bonkableInstance.bonk[0];
                    }
                    else
                    {
                        IsoBadgesPlugin.Log.LogWarning("Could not find a 'Bonkable' object with a valid sound.");
                    }
                }
                return _achievementSound;
            }
        }

        public static void ClearActivePopup()
        {
            _activePopup = null;
        }

        public static void CreateOrUpdateAchievementPopup(MoreBadgesPlugin.CustomBadge badge, int currentProgress)
        {
            if (_activePopup != null)
            {
                if (_activePopup.IsForBadge(badge.name))
                {
                    _activePopup.UpdateProgress(currentProgress);
                    return;
                }
                else
                {
                    _activePopup.CloseImmediate();
                }
            }

            if (AchievementSound != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AchievementSound.Play(soundPosition);
            }

            GameObject canvasGO = new GameObject("AchievementCanvas");
            Object.DontDestroyOnLoad(canvasGO);
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject panelGO = new GameObject("PopupPanel", typeof(RectTransform));
            panelGO.transform.SetParent(canvas.transform, false);
            Image panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            RectTransform panelRect = panelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(1, 1);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.pivot = new Vector2(1, 1);
            panelRect.anchoredPosition = new Vector2(-15, 0);

            GameObject iconGO = new GameObject("Icon", typeof(RectTransform));
            iconGO.transform.SetParent(panelRect.transform, false);
            Image iconImage = iconGO.AddComponent<Image>();
            RectTransform iconRect = iconGO.GetComponent<RectTransform>();

            AchievementPopupUGUI controller = panelGO.AddComponent<AchievementPopupUGUI>();
            _activePopup = controller;
            controller.achievementIcon = iconImage;
            controller.canvasGroup = panelGO.AddComponent<CanvasGroup>();
            controller.rectTransform = panelRect;

            if (ConfigManager.UIStyle.Value == AchievementUIStyle.Compact)
            {
                panelRect.sizeDelta = new Vector2(90, 90);
                iconRect.anchorMin = new Vector2(0.5f, 1);
                iconRect.anchorMax = new Vector2(0.5f, 1);
                iconRect.pivot = new Vector2(0.5f, 1);
                iconRect.sizeDelta = new Vector2(60, 60);
                iconRect.anchoredPosition = new Vector2(0, -5);

                GameObject progressGO = new GameObject("ProgressText", typeof(RectTransform));
                progressGO.transform.SetParent(panelRect.transform, false);
                TextMeshProUGUI progressText = progressGO.AddComponent<TextMeshProUGUI>();
                progressText.font = GameFont;
                progressText.fontSize = 14;
                //progressText.fontStyle = FontStyles.Bold;
                progressText.alignment = TextAlignmentOptions.Center;
                RectTransform progressRect = progressGO.GetComponent<RectTransform>();
                progressRect.anchorMin = new Vector2(0.5f, 0);
                progressRect.anchorMax = new Vector2(0.5f, 0);
                progressRect.pivot = new Vector2(0.5f, 0);
                progressRect.sizeDelta = new Vector2(80, 20);
                progressRect.anchoredPosition = new Vector2(0, 5);

                controller.achievementDescription = progressText;
            }
            else // Default Style
            {
                panelRect.sizeDelta = new Vector2(260, 70);

                iconRect.anchorMin = new Vector2(0, 0.5f);
                iconRect.anchorMax = new Vector2(0, 0.5f);
                iconRect.pivot = new Vector2(0, 0.5f);
                iconRect.sizeDelta = new Vector2(50, 50);
                iconRect.anchoredPosition = new Vector2(10, 0);

                GameObject titleGO = new GameObject("Title", typeof(RectTransform));
                titleGO.transform.SetParent(panelRect.transform, false);
                TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
                titleText.font = GameFont;
                titleText.fontSize = 16;
                //titleText.fontStyle = FontStyles.Bold;
                titleText.color = Color.white;
                titleText.alignment = TextAlignmentOptions.Left;
                RectTransform titleRect = titleGO.GetComponent<RectTransform>();
                titleRect.anchorMin = new Vector2(0, 1);
                titleRect.anchorMax = new Vector2(1, 1);
                titleRect.pivot = new Vector2(0, 1);
                titleRect.sizeDelta = new Vector2(-70, 25);
                titleRect.anchoredPosition = new Vector2(68, -8);

                GameObject descGO = new GameObject("Description", typeof(RectTransform));
                descGO.transform.SetParent(panelRect.transform, false);
                TextMeshProUGUI descText = descGO.AddComponent<TextMeshProUGUI>();
                descText.font = GameFont;
                descText.fontSize = 14;
                descText.alignment = TextAlignmentOptions.TopLeft;
                RectTransform descRect = descGO.GetComponent<RectTransform>();
                descRect.anchorMin = new Vector2(0, 0);
                descRect.anchorMax = new Vector2(1, 0);
                descRect.pivot = new Vector2(0, 0);
                descRect.sizeDelta = new Vector2(-70, 35);
                descRect.anchoredPosition = new Vector2(68, 2);

                controller.achievementTitle = titleText;
                controller.achievementDescription = descText;
            }

            controller.Show(badge, currentProgress);
        }
    }
}
