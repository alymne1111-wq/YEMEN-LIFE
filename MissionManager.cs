using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MissionData
{
    public string titleArabic = "مهمة جديدة";
    [TextArea] public string descriptionArabic = "اتبع التعليمات لإكمال المهمة.";
    [Min(1)] public int requiredObjectives = 1;
    [Min(0)] public int reward = 100;
}

public class MissionManager : MonoBehaviour
{
    public List<MissionData> missions = new List<MissionData>();
    public UnityEvent onMissionStarted;
    public UnityEvent onMissionUpdated;
    public UnityEvent onMissionCompleted;

    public int CurrentMissionIndex { get; private set; } = -1;
    public int CurrentProgress { get; private set; }
    public int PlayerMoney { get; private set; }

    public bool HasActiveMission =>
        CurrentMissionIndex >= 0 &&
        CurrentMissionIndex < missions.Count &&
        CurrentProgress < missions[CurrentMissionIndex].requiredObjectives;

    public string CurrentMissionTitle =>
        HasActiveMission ? missions[CurrentMissionIndex].titleArabic : "لا توجد مهمة نشطة";

    public string CurrentMissionDescription =>
        HasActiveMission ? missions[CurrentMissionIndex].descriptionArabic : "اختر مهمة من لوحة المهام.";

    public string CurrentProgressText =>
        HasActiveMission
            ? CurrentProgress + " / " + missions[CurrentMissionIndex].requiredObjectives
            : "مكتملة";

    public void StartMission(int index)
    {
        if (index < 0 || index >= missions.Count) return;
        CurrentMissionIndex = index;
        CurrentProgress = 0;
        onMissionStarted?.Invoke();
        onMissionUpdated?.Invoke();
    }

    public void RegisterObjectiveProgress(int amount)
    {
        if (!HasActiveMission || amount <= 0) return;

        CurrentProgress = Mathf.Min(
            missions[CurrentMissionIndex].requiredObjectives,
            CurrentProgress + amount);

        onMissionUpdated?.Invoke();

        if (CurrentProgress >= missions[CurrentMissionIndex].requiredObjectives)
        {
            PlayerMoney += missions[CurrentMissionIndex].reward;
            onMissionCompleted?.Invoke();
        }
    }

    public void StartNextMission()
    {
        int next = CurrentMissionIndex + 1;
        if (next < missions.Count) StartMission(next);
    }
}
