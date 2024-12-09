using TMPro;
using UnityEngine;

public class Nhiemvu : MonoBehaviour
{
    public TextMeshProUGUI mission;
    public int enemycount;
    public GameObject[] enemy; // Mảng chứa các enemy
    private bool[] enemyDestroyed; // Mảng để theo dõi trạng thái enemy

    void Start()
    {
        enemycount = 0;
        mission.text = $"Số quái đã tiêu diệt: {enemycount}/3";

        // Khởi tạo mảng trạng thái
        enemyDestroyed = new bool[enemy.Length];
    }

    void Update()
    {
        tieudiet();
        if (enemycount == 3)
        {
            mission.color = Color.green;
        }
    }

    public void tieudiet()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            // Kiểm tra enemy tồn tại và chưa bị đánh dấu tiêu diệt
            if (enemy[i] == null && !enemyDestroyed[i])
            {
                enemycount++;
                enemyDestroyed[i] = true; // Đánh dấu là đã tiêu diệt
                mission.text = $"Số quái đã tiêu diệt: {enemycount}/3";
                Debug.Log($"Enemy {i} đã bị tiêu diệt. Tổng: {enemycount}");
            }
        }
    }
}