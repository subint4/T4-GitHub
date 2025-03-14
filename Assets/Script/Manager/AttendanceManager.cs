using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

[Serializable]
public class AttendanceData
{
    public string Date;
    public bool Attended;
}

public class AttendanceManager
{
    private static readonly string jsonPath = Path.Combine(Application.persistentDataPath, "AttendanceData.json");

    private static readonly Dictionary<int, (int ItemID, string ItemName, int Quantity)> attendanceRewards =
        new Dictionary<int, (int, string, int)>
        {
            { 1,  (101, "Bomb", 1) },
            { 3,  (102, "Rocket", 1) },
            { 5,  (103, "Stun", 1) },
            { 7,  (2, "Diamond", 5) },
            { 10, (3, "Heart", 3) },
            { 14, (1, "Star", 1) },
            { 30, (2, "Diamond", 10) }
        };

    public static List<AttendanceData> LoadAttendanceData()
    {
        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            List<AttendanceData> loadedData = JsonConvert.DeserializeObject<List<AttendanceData>>(json);
            if (loadedData != null)
            {
                Debug.Log("[AttendanceManager] 출석 데이터 로드 완료.");
                return loadedData;
            }
        }

        Debug.LogWarning("[AttendanceManager] 출석 데이터가 존재하지 않습니다.");
        return new List<AttendanceData>();
    }

    public static void SaveAttendanceData(List<AttendanceData> attendanceData)
    {
        string json = JsonConvert.SerializeObject(attendanceData, Formatting.Indented);
        File.WriteAllText(jsonPath, json);
        Debug.Log("[AttendanceManager] 출석 데이터 저장 완료.");
    }

    public static int GetConsecutiveAttendanceDays(List<AttendanceData> attendanceData)
    {
        if (attendanceData.Count == 0) return 0;

        attendanceData.Sort((a, b) => DateTime.Parse(a.Date).CompareTo(DateTime.Parse(b.Date)));

        int consecutiveDays = 1;
        DateTime previousDate = DateTime.Parse(attendanceData[0].Date);

        for (int i = 1; i < attendanceData.Count; i++)
        {
            DateTime currentDate = DateTime.Parse(attendanceData[i].Date);

            if ((currentDate - previousDate).TotalDays == 1)
            {
                consecutiveDays++;
            }
            else if ((currentDate - previousDate).TotalDays > 1)
            {
                break;
            }

            previousDate = currentDate;
        }

        return consecutiveDays;
    }

    public static void MarkAttendance(UserData userData)
    {
        List<AttendanceData> attendanceData = LoadAttendanceData();
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        if (!attendanceData.Exists(a => a.Date == today))
        {
            attendanceData.Add(new AttendanceData { Date = today, Attended = true });
            SaveAttendanceData(attendanceData);
            Debug.Log($"[AttendanceManager] 출석 체크 완료: {today}");

            int consecutiveDays = GetConsecutiveAttendanceDays(attendanceData);
            GrantAttendanceReward(userData, consecutiveDays);
        }
        else
        {
            Debug.LogWarning($"[AttendanceManager] 이미 출석 체크됨: {today}");
        }
    }

    private static void GrantAttendanceReward(UserData userData, int consecutiveDays)
    {
        if (attendanceRewards.TryGetValue(consecutiveDays, out var reward))
        {
            UserDataManager.AddItem(userData, reward.ItemID, reward.ItemName, reward.Quantity);
            Debug.Log($"[AttendanceManager] 출석 보상 지급: {reward.ItemName} {reward.Quantity}개");
        }
        else
        {
            Debug.Log($"[AttendanceManager] {consecutiveDays}일 연속 출석 보상이 없습니다.");
        }
    }
}
