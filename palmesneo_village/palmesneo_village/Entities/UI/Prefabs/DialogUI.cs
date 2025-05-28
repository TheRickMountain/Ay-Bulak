using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;

namespace palmesneo_village
{
    public class DialogUI : PanelUI
    {
        private enum DialogState
        {
            Idle,
            Typing,
            WaitingForInput
        }

        private const float PUNCTUATION_TIME = 0.25f;
        private const float LETTER_TIME = 0.03f;

        private RichTextUI text;
        private List<string> conversation = new();

        private int currentLineIndex = 0;
        private string fullLine = string.Empty;
        private int charIndex = 0;

        private float letterTimer = 0f;
        private float letterDelay = LETTER_TIME;

        private DialogState currentState = DialogState.Idle;

        private ImageUI dialogEndMaker;

        private Tweener tweener;

        private List<SoundEffect> speechSoundFX;

        public DialogUI() : base(PanelStyle.Dialog)
        {
            text = new RichTextUI
            {
                SelfColor = Color.Black,
                LocalPosition = new Vector2(10, 10)
            };
            AddChild(text);

            dialogEndMaker = new ImageUI()
            {
                Texture = ResourcesManager.GetTexture("Sprites", "UI", "dialog_end_marker"),
                Anchor = Anchor.BottomCenter,
                LocalPosition = new Vector2(0, -20),
                Size = Texture.Size,
                IsVisible = false
            };
            AddChild(dialogEndMaker);

            tweener = new Tweener();

            tweener.TweenTo(
                        target: dialogEndMaker,
                        expression: x => dialogEndMaker.LocalPosition,
                        toValue: new Vector2(0, -10),
                        duration: 0.8f)
                .Easing(EasingFunctions.QuadraticInOut)
                .AutoReverse()
                .RepeatForever();

            speechSoundFX = new List<SoundEffect>();
            speechSoundFX.Add(ResourcesManager.GetSoundEffect("SoundEffects", "speech_1"));
            speechSoundFX.Add(ResourcesManager.GetSoundEffect("SoundEffects", "speech_2"));
            speechSoundFX.Add(ResourcesManager.GetSoundEffect("SoundEffects", "speech_3"));
            speechSoundFX.Add(ResourcesManager.GetSoundEffect("SoundEffects", "speech_4"));

            Size = new Vector2(300, 120);
        }

        public override void Update()
        {
            base.Update();

            tweener.Update(Engine.DeltaTime);

            switch (currentState)
            {
                case DialogState.Typing:
                    {
                        dialogEndMaker.IsVisible = false;

                        UpdateTyping();
                    }
                    break;

                case DialogState.WaitingForInput:
                    {
                        dialogEndMaker.IsVisible = true;

                        if (InputBindings.Accept.Pressed)
                        {
                            AdvanceToNextLine();
                        }
                    }
                    break;
            }
        }

        public void Open(List<string> lines)
        {
            if (lines == null || lines.Count == 0)
                return;

            conversation = lines;
            currentLineIndex = 0;
            LoadCurrentLine();
        }

        private void LoadCurrentLine()
        {
            fullLine = conversation[currentLineIndex];
            charIndex = 0;
            letterTimer = 0f;
            letterDelay = 0.03f;
            text.Text = string.Empty;
            currentState = DialogState.Typing;
        }

        private void UpdateTyping()
        {
            letterTimer += Engine.DeltaTime;

            if (charIndex < fullLine.Length && letterTimer >= letterDelay)
            {
                char currentChar = fullLine[charIndex];
                charIndex++;

                text.Text = fullLine.Substring(0, charIndex);

                letterDelay = GetDelayForCharacter(currentChar);
                letterTimer = 0f;

                if (char.IsLetter(currentChar))
                {
                    Calc.Random.Choose(speechSoundFX).Play();
                }

                if (charIndex >= fullLine.Length)
                {
                    currentState = DialogState.WaitingForInput;
                }
            }

            // Скип текста
            if (InputBindings.Accept.Pressed && charIndex < fullLine.Length)
            {
                charIndex = fullLine.Length;
                text.Text = fullLine;

                currentState = DialogState.WaitingForInput;
            }
        }

        private void AdvanceToNextLine()
        {
            currentLineIndex++;

            if (currentLineIndex < conversation.Count)
            {
                LoadCurrentLine();
            }
            else
            {
                EndDialog();
            }
        }

        private void EndDialog()
        {
            conversation.Clear();
            fullLine = string.Empty;
            charIndex = 0;
            currentState = DialogState.Idle;
        }

        private float GetDelayForCharacter(char letter)
        {
            return letter switch
            {
                '.' or '!' or '?' => PUNCTUATION_TIME,
                _ => LETTER_TIME
            };
        }
    }
}
