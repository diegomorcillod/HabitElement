using System;

[Serializable]
public class Habit
{
    public string id;
    public string name;
    public string lastDoneDate; // ISO "yyyy-MM-dd"

    public Habit(string name)
    {
        this.id = Guid.NewGuid().ToString();
        this.name = name;
        this.lastDoneDate = "";
    }

    public static string Today() => DateTime.UtcNow.ToString("yyyy-MM-dd");
    public bool IsDoneToday() => lastDoneDate == Today();

    public void SetDoneToday(bool done)
    {
        if (done) lastDoneDate = Today();
        else if (IsDoneToday()) lastDoneDate = ""; // permite desmarcar el mismo día
    }
}
