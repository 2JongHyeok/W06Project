// WorldAnalysisLogger.cs
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;       // 파일 입출력을 위해 꼭 필요합니다!
using System;         // 날짜와 시간을 위해 꼭 필요합니다!
using System.Text;    // 긴 텍스트를 효율적으로 만들기 위해 필요합니다!

public class WorldAnalysisLogger : MonoBehaviour
{
    [Header("분석 대상")]
    [Tooltip("월드에 있는 메인 타일맵을 연결해주세요.")]
    [SerializeField] private Tilemap worldTilemap;

    [Header("설정")]
    [Tooltip("월드 생성이 끝날 때까지 기다릴 시간 (초)")]
    [SerializeField] private float waitSecondsBeforeAnalysis = 2.0f;
    
    // ✨ [수정] 파일 이름을 더 직관적인 .txt 파일로 변경
    [Tooltip("로그 파일 이름")]
    [SerializeField] private string logFileName = "World_Analysis_Report.txt";

    void Start()
    {
        if (worldTilemap == null)
        {
            Debug.LogError("분석할 World Tilemap이 연결되지 않았습니다!");
            return;
        }
        StartCoroutine(AnalyzeAfterDelay());
    }

    private IEnumerator AnalyzeAfterDelay()
    {
        Debug.Log($"{waitSecondsBeforeAnalysis}초 후 월드 광물 분석을 시작합니다...");
        yield return new WaitForSeconds(waitSecondsBeforeAnalysis);
        
        AnalyzeAndLogWorldOres();
    }

    public void AnalyzeAndLogWorldOres()
    {
        Debug.Log("월드 분석 중...");

        Dictionary<string, int> oreCounts = new Dictionary<string, int>();
        int totalOreCount = 0;

        worldTilemap.CompressBounds();
        foreach (var pos in worldTilemap.cellBounds.allPositionsWithin)
        {
            if (worldTilemap.HasTile(pos))
            {
                TileBase tile = worldTilemap.GetTile(pos);
                string tileName = tile.name;

                if (oreCounts.ContainsKey(tileName))
                {
                    oreCounts[tileName]++;
                }
                else
                {
                    oreCounts[tileName] = 1;
                }
                totalOreCount++;
            }
        }

        Debug.Log($"분석 완료! 총 {totalOreCount}개의 타일을 발견했습니다.");
        
        // 분석 결과를 새로운 리포트 형식으로 파일에 저장합니다.
        LogResultsAsReport(oreCounts, totalOreCount);
    }

    // ✨ --- 이 함수가 완전히 새로워졌습니다! --- ✨
    /// <summary>
    /// 분석 결과를 사람이 읽기 쉬운 리포트 형식으로 .txt 파일에 누적 기록합니다.
    /// </summary>
    private void LogResultsAsReport(Dictionary<string, int> counts, int total)
    {
        string filePath = Path.Combine(Application.dataPath, logFileName);
        
        // StringBuilder는 여러 줄의 텍스트를 만들 때 훨씬 효율적입니다.
        StringBuilder report = new StringBuilder();

        // --- 리포트 내용 생성 시작 ---
        report.AppendLine("==============================================================");
        report.AppendLine($" 분석 시간: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine("--------------------------------------------------------------");
        report.AppendLine();
        report.AppendLine("      [ 월드 생성 분석 리포트 ]");
        report.AppendLine();
        report.AppendLine($"  > 발견된 총 광물 타일 수: {total:N0}개"); // N0는 1,000단위 쉼표 추가
        report.AppendLine();
        report.AppendLine("  ▼ 광물 분포 상세 ▼");

        // 개수가 많은 순서대로 정렬해서 보여줍니다.
        var sortedCounts = counts.OrderByDescending(pair => pair.Value);

        foreach (var oreEntry in sortedCounts)
        {
            string oreName = oreEntry.Key;
            int count = oreEntry.Value;
            float percentage = (total > 0) ? (float)count / total * 100f : 0f;
            report.AppendLine($"    - {oreName,-20} : {count,8:N0}개 ({percentage,6:F2}%)");
        }
        report.AppendLine();

        // 가장 흔한 광물을 찾아 요약해줍니다.
        if (sortedCounts.Any())
        {
            var topOre = sortedCounts.First();
            report.AppendLine("  [ 요약 ]");
            report.AppendLine($"    이번 월드에서 가장 흔한 광물은 '{topOre.Key}' 이며,");
            report.AppendLine($"    총 {topOre.Value:N0}개가 발견되어 전체의 약 {((float)topOre.Value / total * 100f):F2}%를 차지합니다.");
        }
        else
        {
            report.AppendLine("  [ 요약 ] 발견된 광물이 없습니다.");
        }
        
        report.AppendLine();
        report.AppendLine("==============================================================");
        report.AppendLine(); // 리포트 사이에 한 줄 공백 추가

        // --- 리포트 내용 생성 끝 ---

        // 기존 파일의 맨 끝에 새로운 리포트를 추가합니다.
        File.AppendAllText(filePath, report.ToString());

        Debug.Log($"분석 리포트가 {filePath} 파일에 저장되었습니다.");
    }
}