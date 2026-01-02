using UnityEngine;
using UnityEditor;
public class Qtils : MonoBehaviour
{
    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float t = Mathf.InverseLerp(fromMin, fromMax, value);
        return Mathf.Lerp(toMin, toMax, t);
    }

    /// <summary>
    /// Remaps a value from one range to another using Vector2s for min/max.
    /// from.x = min, from.y = max. to.x = min, to.y = max.
    /// </summary>
    public static float Remap(float value, Vector2 from, Vector2 to)
    {
        if (Mathf.Approximately(from.x, from.y)) return (to.x + to.y) * 0.5f;
        return (value - from.x) / (from.y - from.x) * (to.y - to.x) + to.x;
    }
    
    public static Vector2 Remap(Vector2 value, Vector2 from, Vector2 to)
    {
        float RemapComponent(float v)
        {
            if (Mathf.Approximately(from.x, from.y)) return (to.x + to.y) * 0.5f;
            return (v - from.x) / (from.y - from.x) * (to.y - to.x) + to.x;
        }

        return new Vector2(RemapComponent(value.x), RemapComponent(value.y));
    }

    public static bool PointInRange(Vector2 point, Vector2 from, Vector2 to)
    {
        if(point.x > from.x && point.x < to.x && point.y > from.y && point.y < to.y) return true;
        return false;
    }

    
    public static bool IsFinite(Vector2 v) =>
        !(float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsInfinity(v.x) || float.IsInfinity(v.y));

    public static Vector2[] ResamplePoints(Vector2[] positions, int resolution)
    {
        if (positions == null || positions.Length < 2 || resolution <= 0)
            return new Vector2[0];

        float totalLength = 0f;
        for (int i = 1; i < positions.Length; i++)
            totalLength += Vector2.Distance(positions[i], positions[i - 1]);

        float segmentLength = totalLength / resolution;
        Vector2[] result = new Vector2[resolution];
        float distanceWalked = 0;
        int index = 1;

        for (int i = 1; i < positions.Length && index < resolution; i++)
        {
            distanceWalked += Vector2.Distance(positions[i], positions[i - 1]);
            if (distanceWalked >= segmentLength * index)
            {
                result[index - 1] = positions[i];
                index++;
            }
        }

        result[resolution - 1] = positions[positions.Length - 1];
        for (int k = index; k < resolution; k++)
        {
            result[k] = positions[positions.Length - 1];
        }

        return result;
    }
    
    
    public static Rect ScreenRect(RectTransform rt, Camera cam)
    {
        var w = new Vector3[4];
        rt.GetWorldCorners(w);
        for (int i = 0; i < 4; i++) w[i] = cam ? cam.WorldToScreenPoint(w[i]) : (Vector3)RectTransformUtility.WorldToScreenPoint(null, w[i]);
        var min = new Vector2(Mathf.Min(w[0].x, w[2].x), Mathf.Min(w[0].y, w[2].y));
        var max = new Vector2(Mathf.Max(w[0].x, w[2].x), Mathf.Max(w[0].y, w[2].y));
        return new Rect(min, max - min);
    }

    public static bool UIOverlapAA(RectTransform a, RectTransform b, Camera uiCam)
    {
        return ScreenRect(a, uiCam).Overlaps(ScreenRect(b, uiCam));
    }

    public static string TimeObjDateTranslatorShort(TimeObj timeObj)
    {
        return $"{timeObj.month}.{timeObj.day}.{timeObj.year}";
    }
    
    public static string TimeObjTimeTranslatorShort(TimeObj timeObj)
    {
        string appendix = timeObj.hour/12 == 0 ? "AM" : "PM";
        string minPre = "";
        int hour = timeObj.hour;
        if(timeObj.hour >12) hour = timeObj.hour - 12;
        if(timeObj.minute < 10) minPre = "0";
        return $"{hour}:{minPre}{timeObj.minute}{appendix}";
    }
    
    public static string TimeObjDateTranslatorLong(TimeObj timeObj)
    {
        string monthLong = "";
        switch (timeObj.month)
        {
            case 1:
                monthLong = "January";
                break;
            case 2:
                monthLong = "February";
                break;
            case 3:
                monthLong = "March";
                break;
            case 4:
                monthLong = "April";
                break;
            case 5:
                monthLong = "May";
                break;
            case 6:
                monthLong = "June";
                break;
            case 7:
                monthLong = "July";
                break;
            case 8:
                monthLong = "August";
                break;
            case 9:
                monthLong = "September";
                break;
            case 10:
                monthLong = "October";
                break;
            case 11:
                monthLong = "November";
                break;
            case 12:
                monthLong = "December";
                break;
        }
        
        string dayApp = "";
        switch (timeObj.day%10)
        {
            case 1:
                if(timeObj.day >= 11 && timeObj.day <= 19)
                {
                    dayApp = "th";
                    break;
                }
                dayApp = "st";
                break;
            case 2:
                if(timeObj.day >= 11 && timeObj.day <= 19)
                {
                    dayApp = "th";
                    break;
                }
                dayApp = "nd";
                break;
            case 3:
                if(timeObj.day >= 11 && timeObj.day <= 19)
                {
                    dayApp = "th";
                    break;
                }
                dayApp = "rd";
                break;
            default:
                dayApp = $"th";
                break;
        }
        return $"{monthLong} {timeObj.day}{dayApp}, {timeObj.year}";
    }


    public static TimeObj cleanTimeObj(TimeObj timeObj)
    {
        while (timeObj.second > 59)
        {
            timeObj.second = 1;
            timeObj.minute++;
        }

        while (timeObj.minute > 59)
        {
            timeObj.minute = 1;
            timeObj.hour++;
        }

        while (timeObj.hour > 24)
        {
            timeObj.hour = 1;
            timeObj.day++;
        }
        // Normalize months first so they're in 1–12
        while (timeObj.month < 1)
        {
            timeObj.month += 12;
            timeObj.year -= 1;
        }
        while (timeObj.month > 12)
        {
            timeObj.month -= 12;
            timeObj.year += 1;
        }

        // Now fix days going backwards
        while (timeObj.day < 1)
        {
            // Step back one month
            timeObj.month--;
            if (timeObj.month < 1)
            {
                timeObj.month = 12;
                timeObj.year--;
            }

            int daysInPrevMonth = DaysInMonth(timeObj.year, timeObj.month);
            timeObj.day += daysInPrevMonth;
        }

        // And days overflowing forward
        while (true)
        {
            int daysInThisMonth = DaysInMonth(timeObj.year, timeObj.month);
            if (timeObj.day <= daysInThisMonth)
                break;

            timeObj.day -= daysInThisMonth;
            timeObj.month++;

            if (timeObj.month > 12)
            {
                timeObj.month = 1;
                timeObj.year++;
            }
        }

        while (timeObj.month > 12)
        {
            timeObj.month = 1 + 12 - Mathf.Abs(timeObj.month%12);
            timeObj.year += 1;
        }
        return timeObj;
    }
    
    private static bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
    }

    public static int DaysInMonth(int year, int month)
    {
        switch (month)
        {
            case 1:  return 31; // Jan
            case 2:  return IsLeapYear(year) ? 29 : 28; // Feb
            case 3:  return 31;
            case 4:  return 30;
            case 5:  return 31;
            case 6:  return 30;
            case 7:  return 31;
            case 8:  return 31;
            case 9:  return 30;
            case 10: return 31;
            case 11: return 30;
            case 12: return 31;
            default: return 31; // fallback, shouldn't hit if you normalize month elsewhere
        }
    }

    
    
    public static bool IsPlayingSafe()
    {
#if UNITY_EDITOR
        return UnityEditor.EditorApplication.isPlaying;
#else
    return true; // in player builds, always “playing”
#endif
    }
}


[System.Serializable]
public class LerpObj
{
    public Transform visTransform;
    public Transform startPos;
    public Transform endPos;
    public float time = -1;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
}

[System.Serializable]
public class LerpVal
{
    public float startValue;
    public float endValue;
    public float value;
    public float time = -1;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
}

[System.Serializable]
public struct TimeObj
{
    public int month;
    public int day;
    public int year;

    public int hour;
    public int minute;
    public int second;

    public TimeObj(TimeObj timeObj)
    {
        month = timeObj.month;
        day = timeObj.day;
        year = timeObj.year;
        hour = timeObj.hour;
        minute = timeObj.minute;
        second = timeObj.second;
    }
    public static TimeObj FindEarlierTime(bool onlyAffectDay, TimeObj startTime, int varienceMin,int varienceMax)
    {
        TimeObj timeObj = new TimeObj();
        if (onlyAffectDay)
        {
            timeObj.year = startTime.year;
            timeObj.month = startTime.month;
            timeObj.day = startTime.day - Random.Range(varienceMin, varienceMax);
        }
        else
        {
            timeObj.year = startTime.year - Random.Range(varienceMin, varienceMax);
            timeObj.month = Random.Range(1, 13);

            int maxDay = Qtils.DaysInMonth(timeObj.year, timeObj.month);
            timeObj.day = Random.Range(1, maxDay + 1); // Unity int Range: min inclusive, max exclusive
        }

        TimeObj cleanTime = Qtils.cleanTimeObj(timeObj);
        return cleanTime;
    }
    public static TimeObj FindLaterTime(bool onlyAffectDay, TimeObj startTime, int varienceMin,int varienceMax)
    {
        TimeObj timeObj = new TimeObj();
        if (onlyAffectDay)
        {
            timeObj.year = startTime.year;
            timeObj.month = startTime.month;
            timeObj.day = startTime.day + Random.Range(varienceMin, varienceMax);
        }
        else
        {
            timeObj.year = startTime.year + Random.Range(varienceMin, varienceMax);
            timeObj.month = Random.Range(1, 13);

            int maxDay = Qtils.DaysInMonth(timeObj.year, timeObj.month);
            timeObj.day = Random.Range(1, maxDay + 1);
        }

        TimeObj cleanTime = Qtils.cleanTimeObj(timeObj);
        return cleanTime;
    }
}