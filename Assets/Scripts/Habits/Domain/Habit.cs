using System;

[Serializable]
public class Habit
{
    public string id;
    public string name;
    public string lastDoneDate; // ISO "yyyy-MM-dd"
    public int streak;

    public Habit(string name)
    {
        this.id = Guid.NewGuid().ToString();
        this.name = name;
        this.lastDoneDate = "";
        this.streak = 0;
    }

    public static string Today() => DateTime.UtcNow.ToString("yyyy-MM-dd");
    public static string Yesterday() => DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
    public bool IsDoneToday() => lastDoneDate == Today();

    public void SetDoneToday(bool done)
    {
        var today = Today();
        if (done)
        {
            if (lastDoneDate == today) return; // Ya estaba marcado hoy
            if (lastDoneDate == Yesterday()) streak = Math.Max(0, streak + 1);
            else streak = 1;
            lastDoneDate = today;
        }
        else
        {
            if (lastDoneDate == today)
            {
                streak = Math.Max(0, streak - 1);
                lastDoneDate = "";
            }
        }
    }
}
