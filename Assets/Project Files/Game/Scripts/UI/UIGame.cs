﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIGame : UIPage
    {
        [SerializeField] RectTransform safeAreaRectTransform;
        [SerializeField] CurrencyUIPanelSimple coinsPanel;
        [SerializeField] UILevelQuitPopUp quitPopUp;
        [SerializeField] UILevelNumberText levelNumberText;
        [SerializeField] TimerVisualiser gameplayTimer;

        [SerializeField] PUUIController powerUpsUIController;
        public PUUIController PowerUpsUIController => powerUpsUIController;

        [SerializeField] UILevelQuitPopUp exitPopUp;
        [SerializeField] Button exitButton;
        [SerializeField] UIFadeAnimation exitButtonFadeAnimation;

        [SerializeField] GameObject devOverlay;

        [LineSpacer("Tutorial")]
        [SerializeField] GameObject tutorialPanelObject;
        [SerializeField] TextMeshProUGUI tutorialTitleText;
        [SerializeField] TextMeshProUGUI tutorialDescriptionText;
        [SerializeField] Button tutorialSkipButton;

        public TimerVisualiser GameplayTimer => gameplayTimer;

        public override void Init()
        {
            coinsPanel.Init();
            exitButton.onClick.AddListener(ShowExitPopUp);
            exitButtonFadeAnimation.Hide(immediately: true);

            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
            NotchSaveArea.RegisterRectTransform((RectTransform)tutorialPanelObject.transform);

            DevPanelEnabler.RegisterPanel(devOverlay);

            tutorialSkipButton.onClick.AddListener(OnTutorialSkipButtonClicked);
        }

        private void OnEnable()
        {
            exitPopUp.OnConfirmExitEvent += ExitPopUpConfirmExitButton;
            exitPopUp.OnCancelExitEvent += ExitPopCloseButton;
        }

        private void OnDisable()
        {
            exitPopUp.OnConfirmExitEvent -= ExitPopUpConfirmExitButton;
            exitPopUp.OnCancelExitEvent += ExitPopCloseButton;
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            coinsPanel.Activate();
            exitButtonFadeAnimation.Show();

            UILevelNumberText.Show();

            IntToggle timer = LevelController.Level.Timer;
            if (timer.Enabled)
            {
                gameplayTimer.gameObject.SetActive(true);
                gameplayTimer.Show(LevelController.GameplayTimer);
            }
            else
            {
                gameplayTimer.gameObject.SetActive(false);
            }

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            coinsPanel.Disable();
            exitButtonFadeAnimation.Hide();

            UILevelNumberText.Hide();

            IntToggle timer = LevelController.Level.Timer;
            if (timer.Enabled)
            {
                gameplayTimer.Hide();
            }

            UIController.OnPageClosed(this);
        }

        public void UpdateLevelNumber(int levelNumber)
        {
            levelNumberText.UpdateLevelNumber(levelNumber);
        }
        #endregion

        public void ShowExitPopUp()
        {
            if(!LivesSystem.InfiniteMode)
            {
                exitPopUp.Show();
            }
            else
            {
                ExitPopUpConfirmExitButton();
            }

            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        public void ExitPopCloseButton()
        {
            exitPopUp.Hide();
        }

        public void ExitPopUpConfirmExitButton()
        {
            LivesSystem.UnlockLife(true);

            UIController.HidePage<UIGame>();

            GameController.ReturnToMenu();

            exitPopUp.Hide();
        }

        #region Tutorial
        public void ActivateTutorial()
        {
            tutorialPanelObject.SetActive(true);

            exitButton.gameObject.SetActive(false);
            levelNumberText.gameObject.SetActive(false);

            powerUpsUIController.HidePanels();
        }

        public void DisableTutorial()
        {
            tutorialPanelObject.SetActive(false);

            exitButton.gameObject.SetActive(true);
            levelNumberText.gameObject.SetActive(true);
        }

        public void SetTutorialText(string title, string description)
        {
            tutorialTitleText.text = title;
            tutorialDescriptionText.text = description;

            tutorialTitleText.transform.localScale = Vector3.one * 0.6f;
            tutorialTitleText.transform.DOScale(1.0f, 0.3f).SetEasing(Ease.Type.BackOut);

            tutorialDescriptionText.transform.localScale = Vector3.one * 0.6f;
            tutorialDescriptionText.transform.DOScale(1.0f, 0.3f).SetEasing(Ease.Type.BackOut);
        }

        private void OnTutorialSkipButtonClicked()
        {
            ITutorial tutorial = TutorialController.GetTutorial(TutorialID.FirstLevel);
            if(tutorial != null)
            {
                FirstLevelTutorial firstLevelTutorial = (FirstLevelTutorial)tutorial;
                firstLevelTutorial.OnSkipButtonClicked();
            }
        }
        #endregion

        #region Development

        public void ReloadDev()
        {
            GameController.ReplayLevel();

            UIController.DisablePage<UIGame>();
            UIController.ShowPage<UIGame>();
        }

        public void HideDev()
        {
            devOverlay.SetActive(false);
        }

        public void OnLevelInputUpdatedDev(string newLevel)
        {
            int level = -1;

            if (int.TryParse(newLevel, out level))
            {
                LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
                levelSave.DisplayLevelIndex = Mathf.Clamp((level - 1), 0, int.MaxValue);
                levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;

                GameController.ReplayLevel();

                UIController.DisablePage<UIGame>();
                UIController.ShowPage<UIGame>();
            }
        }

        public void PrevLevelDev()
        {
            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            levelSave.DisplayLevelIndex = Mathf.Clamp(levelSave.DisplayLevelIndex - 1, 0, int.MaxValue);
            levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;

            GameController.ReplayLevel();

            UIController.DisablePage<UIGame>();
            UIController.ShowPage<UIGame>();
        }

        public void NextLevelDev()
        {
            LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
            levelSave.DisplayLevelIndex = levelSave.DisplayLevelIndex + 1;
            levelSave.RealLevelIndex = levelSave.DisplayLevelIndex;

            GameController.ReplayLevel();

            UIController.DisablePage<UIGame>();
            UIController.ShowPage<UIGame>();
        }

        public void CompleteDev()
        {
            StartCoroutine(CompleteCoroutine());
        }

        private IEnumerator CompleteCoroutine()
        {
            while (GameController.IsGameActive)
            {
                PUController.UsePowerUp(PUType.Hint);

                yield return new WaitForSeconds(0.2f);
            }
            
        }

        #endregion
    }
}
