/// <summary>
/// Button类（各种Button状态）
/// </summary>
public class MyButton {
    public bool isPressing = false;
    public bool onPressed = false;
    public bool onReleased = false;
    public bool isDelaying = false;
    public bool isExtending = false;

    public float extendingDuration = 0.15f;
    public float delayingDuration = 0.15f;

    private bool lastState;
    private bool currentState;

    private MyTimer extendingTimer = new MyTimer();
    private MyTimer delayingTimer = new MyTimer();

    public void Tick(bool input)
    {
        extendingTimer.Tick();
        delayingTimer.Tick();

        currentState = input;
        isPressing = currentState;

        onPressed = false;
        onReleased = false;

        if (currentState!=lastState)
        {
            if (currentState)
            {
                onPressed = true;
                StartTimer(delayingTimer, delayingDuration);
            }
            else
            {
                onReleased = true;
                StartTimer(extendingTimer, extendingDuration);
            }
        }
        isExtending = (extendingTimer.state == MyTimer.STATE.RUN);
        isDelaying = (delayingTimer.state == MyTimer.STATE.RUN);
        lastState = currentState;
    }

    private void StartTimer(MyTimer timer, float duration)
    {
        timer.duration = duration;
        timer.Go();
    }

}
