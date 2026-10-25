using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class InstructionsPanelWithConfirmation : InstructionsPanel
{
    [SerializeField]
    private GameButton confirmButton;


    protected override void Awake()
    {
        base.Awake();
        if (this.confirmButton == null)
        {
            Debug.LogError($"[InstructionsPanelWithConfirmation] {gameObject.name}: confirmButton reference is not set in the inspector.");
        }

        if (hideOnAwake)
        {
            confirmButton.gameObject.SetActive(false);
        }
    }


    protected override void ShowInstant()
    {
        base.ShowInstant();
        confirmButton.gameObject.SetActive(true);
    }

    protected override void HideInstant()
    {
        base.HideInstant();
        confirmButton.gameObject.SetActive(false);
    }

    public async UniTask ShowAndWaitForConfirmation(bool doResizeBackPanel = true)
    {
        await Show(doResizeBackPanel);
        await confirmButton.WaitForButtonPress();

        Debug.Log($"[InstructionsPanelWithConfirmation] {gameObject.name} confirmed by user.");
        await Hide();
    }

    // Shows the panel and keeps it up until EITHER the confirm button is pressed OR `dismiss` is
    // cancelled (e.g. the participant reached the target the prompt refers to), then hides it —
    // whichever happens first. This lets a prompt be dismissible by the user's button press AND
    // auto-dismissed by game logic, without the button being a gate. Never throws.
    public async UniTask ShowUntilDismissed(CancellationToken dismiss, bool doResizeBackPanel = true)
    {
        await Show(doResizeBackPanel);
        await UniTask.WhenAny(confirmButton.WaitForButtonPress(), UniTask.WaitUntilCanceled(dismiss));
        await Hide();
    }


}
