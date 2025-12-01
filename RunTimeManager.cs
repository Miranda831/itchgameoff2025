using UnityEngine;
using TMPro;

public class RunTimeManager : MonoBehaviour
{
    public static RunTimeManager Instance;

    public TMP_Text currentTimeText;
    public TMP_Text[] timeSlots = new TMP_Text[5];

    private const int MaxRecords = 5;
    private float[] _bestTimes = new float[MaxRecords];
    private int _recordCount = 0;

    private bool _isTiming = false;
    private float _currentTime = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < MaxRecords; i++)
            _bestTimes[i] = -1f;

        UpdateBoardUI();
    }

    void Update()
    {
        if (!_isTiming) return;

        _currentTime += Time.deltaTime;

        if (currentTimeText != null)
            currentTimeText.text = FormatTime(_currentTime);
    }

    public void StartRun()
    {
        _currentTime = 0f;
        _isTiming = true;
    }

    public void StopRunAndRecord()
    {
        if (!_isTiming) return;

        _isTiming = false;
        AddNewRecord(_currentTime);
        UpdateBoardUI();
    }

    private void AddNewRecord(float newTime)
    {
        if (_recordCount < MaxRecords)
        {
            _bestTimes[_recordCount] = newTime;
            _recordCount++;
        }
        else
        {
            int worstIndex = 0;
            float worstTime = _bestTimes[0];

            for (int i = 1; i < MaxRecords; i++)
            {
                if (_bestTimes[i] > worstTime)
                {
                    worstTime = _bestTimes[i];
                    worstIndex = i;
                }
            }

            if (newTime >= worstTime) return;

            _bestTimes[worstIndex] = newTime;
        }

        System.Array.Sort(_bestTimes, 0, _recordCount);
    }

    private void UpdateBoardUI()
    {
        for (int i = 0; i < timeSlots.Length; i++)
        {
            if (timeSlots[i] == null) continue;

            if (i < _recordCount && _bestTimes[i] >= 0f)
                timeSlots[i].text = (i + 1) + ". " + FormatTime(_bestTimes[i]);
            else
                timeSlots[i].text = (i + 1) + ". --:--";
        }
    }

    private string FormatTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60f);
        float seconds = t - minutes * 60;
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }

    public void ClearRecords()
    {
        _recordCount = 0;
        for (int i = 0; i < MaxRecords; i++)
            _bestTimes[i] = -1f;

        UpdateBoardUI();
    }
}
