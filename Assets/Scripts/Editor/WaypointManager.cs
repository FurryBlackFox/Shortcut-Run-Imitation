using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaypointManager : EditorWindow
{
    [MenuItem("Tools/Waypoint Manager")]
    public static void Open()
    {
        GetWindow<WaypointManager>();
    }

    public Transform waypointRoot;

    private GUIStyle centeredStyle;

    private void OnGUI()
    {
        var obj = new SerializedObject(this);
        
        centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;
        
        DrawToggleButtons();
        
        DrawRootButtons(obj);
        
        if (waypointRoot)
        {
            DrawWaypointButtons();
        }
        else
        {
            EditorGUILayout.HelpBox("Please assign a root transform or create a new one", MessageType.Warning);
        }

        obj.ApplyModifiedProperties();
    }

    #region Buttons

    private void DrawToggleButtons()
    {
        EditorGUILayout.BeginVertical("box");
        
        GUILayout.Label("Toggles", centeredStyle);
        
        GameDataKeeper.S.WaypointSettings.showWaypointNames =
            GUILayout.Toggle(GameDataKeeper.S.WaypointSettings.showWaypointNames, "Show Waypoints Names");
        Waypoint.showWaypointsNames = GameDataKeeper.S.WaypointSettings.showWaypointNames;

        GameDataKeeper.S.WaypointSettings.showWaypointBorders =
            GUILayout.Toggle(GameDataKeeper.S.WaypointSettings.showWaypointBorders, "Show Waypoints Borders");
        Waypoint.showWaypointBorders = GameDataKeeper.S.WaypointSettings.showWaypointBorders;
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawRootButtons(SerializedObject obj)
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.PropertyField(obj.FindProperty("waypointRoot"));
        if(GUILayout.Button("New"))
        {
            CreateWaypointRoot("New Waypoint Root");
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Generate Bot Route"))
        {
            CreateWaypointRoot("Generated Waypoint Root");
            GenerateWaypointRoute();
            CalculateDistancesToFinish();
            GenerateBranches();
            CalculateAllBranchesCost();
        }
    }
    
    private void DrawWaypointButtons()
    {
        GUILayout.Label(waypointRoot.gameObject.name, centeredStyle);
        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("Create New Waypoint"))
        {
            CreateWaypoint();
        }
        
        if (GUILayout.Button("Calculate Branches Cost"))
        {
            CalculateAllBranchesCost();
        }
        if (GUILayout.Button("Clear Branches"))
        {
            ClearBranches();
        }
        if (GUILayout.Button("Generate Branches"))
        {
            CalculateDistancesToFinish();
            GenerateBranches();
            CalculateAllBranchesCost();
            SortBranches();
        }

        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(Selection.activeGameObject.name, centeredStyle);
            var selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
            
            if (GUILayout.Button("Create Brunch"))
            {
                CreateBrunch(selectedWaypoint);
            }
            
            if (GUILayout.Button("Calculate Branches Cost"))
            {
                CalculateCurrentWaypointBranchesCost(selectedWaypoint);
            }
            
            if (GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore(selectedWaypoint);
            }

            if (GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter(selectedWaypoint);
            }
            
            if (GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint(selectedWaypoint);
            }

            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndVertical();
    }

    #endregion


    #region RootMethods

    private void CreateWaypointRoot(string rootName)
    {
        var newWaypointRoot = new GameObject(rootName);
        newWaypointRoot.tag = "WaypointRoot";
        Selection.activeObject = newWaypointRoot;
        waypointRoot =  newWaypointRoot.transform;
    }
    
    private void GenerateWaypointRoute()
    {
        var startPlatform = GameObject.FindGameObjectWithTag("Start");
        if (startPlatform == null)
        {
            Debug.LogError("There is no Start Platform");
            return;
        }
        
        var waypoints = FindObjectsOfType<Waypoint>().ToList();
        if(waypoints.Count == 0)
        {
            Debug.LogError("There is no Waypoints");
            return;
        }
        
        var lastWaypoints = new List<Waypoint>();
        foreach (var waypoint in waypoints)
        {
            if (!waypoint.nextWaypoint && waypoint.branches.Count == 0)
                lastWaypoints.Add(waypoint);
        }
        
        foreach (var lastWaypoint in lastWaypoints)
        {
            foreach (var waypoint in waypoints)
            {
                if (waypoint == lastWaypoint)
                    continue;
                var sqrDistance = (lastWaypoint.transform.position - waypoint.transform.position).sqrMagnitude;
                if (sqrDistance < 1f)
                {
                    if (waypoint.nextWaypoint)
                    {
                        lastWaypoint.nextWaypoint = waypoint.nextWaypoint;
                        lastWaypoint.nextWaypoint.previousWaypoint = lastWaypoint;
                    }

                    lastWaypoint.branches = waypoint.branches;
                    foreach (var branch in lastWaypoint.branches)
                    {
                        branch.waypoint.previousWaypoint = lastWaypoint;
                    }
                    
                    waypoint.gameObject.SetActive(false);
                }
            }
        }
        
        waypoints = FindObjectsOfType<Waypoint>().ToList();
        var newWaypoints = new List<Waypoint>();
        
        foreach (var waypoint in waypoints)
        {
            if (!waypoint.gameObject.activeInHierarchy)
                continue;
            var newWaypointGo = CreateWaypointGo();
            var newWaypoint = newWaypointGo.GetComponent<Waypoint>();
            newWaypoints.Add(newWaypoint);
        }
        
        for(var i = 0; i < waypoints.Count; i++)
        {
            CopyTransform(newWaypoints[i].transform, waypoints[i].transform);
            if (waypoints[i].nextWaypoint)
            {
                var index = waypoints.IndexOf(waypoints[i].nextWaypoint);
                if(index == -1)
                    Debug.LogError($"waypoint {i} next");
                else
                    newWaypoints[i].nextWaypoint = newWaypoints[index];
            }
            
            if (waypoints[i].previousWaypoint)
            {
                var index = waypoints.IndexOf(waypoints[i].previousWaypoint);
                if(index == -1)
                    Debug.LogError($"waypoint {i} prev");
                else
                    newWaypoints[i].previousWaypoint = newWaypoints[index];
            }
        
            if (waypoints[i].branches.Count > 0)
            {
                newWaypoints[i].branches = new List<Branch>();
                foreach (var branch in waypoints[i].branches)
                {
                    var tempBranch = branch;
                    var index = waypoints.IndexOf(branch.waypoint);
                    tempBranch.waypoint = newWaypoints[index];
                    newWaypoints[i].branches.Add(tempBranch);
                }
            }
        }
        
        foreach (var waypoint in waypoints)
        {
            waypoint.gameObject.SetActive(false);
            waypoint.transform.parent.gameObject.SetActive(false);
        }
    }

    #endregion

    #region WaypointsMethods

     private void CopyTransform(Transform to, Transform from)
    {
        to.position = from.position;
        to.rotation = from.rotation;
    }
    

    private GameObject CreateWaypointGo(string prefixName = "")
    {
        var waypointGO = new GameObject(prefixName + "Waypoint " + waypointRoot.childCount,typeof(Waypoint));
        waypointGO.transform.SetParent(waypointRoot, false);
        waypointGO.tag = "Waypoint";
        return waypointGO;
    }
    
    private void SetWaypointPos(Transform originWaypoint, Transform newWaypoint)
    {
        newWaypoint.position = originWaypoint.position;
        newWaypoint.forward = originWaypoint.forward;
    }

    private Waypoint CreateWaypoint()
    {
        var waypointGO = CreateWaypointGo();
        
        var waypointsCount = waypointRoot.childCount;
        var waypoint = waypointGO.GetComponent<Waypoint>();
        
        if (waypointsCount > 1)
        {
            waypoint.previousWaypoint = waypointRoot.GetChild(waypointsCount - 2).GetComponent<Waypoint>();
            waypoint.previousWaypoint.nextWaypoint = waypoint;
            SetWaypointPos(waypoint.previousWaypoint.transform, waypoint.transform);
        }

        Selection.activeGameObject = waypointGO;
        return waypoint;
    }

    private Waypoint CreateWaypointBefore(Waypoint selectedWaypoont)
    {
        var waypointGO = CreateWaypointGo();
        SetWaypointPos(selectedWaypoont.transform, waypointGO.transform);
        var newWaypoint = waypointGO.GetComponent<Waypoint>();
        
        if (selectedWaypoont.previousWaypoint)
        {
            selectedWaypoont.previousWaypoint.nextWaypoint = newWaypoint;
            newWaypoint.previousWaypoint = selectedWaypoont.previousWaypoint;
        }
        selectedWaypoont.previousWaypoint = newWaypoint;
        newWaypoint.nextWaypoint = selectedWaypoont;
        
        newWaypoint.transform.SetSiblingIndex(selectedWaypoont.transform.GetSiblingIndex());
        Selection.activeGameObject = waypointGO;
        return newWaypoint;
    }

    private Waypoint CreateWaypointAfter(Waypoint selectedWaypoont)
    {
        var waypointGO = CreateWaypointGo();
        SetWaypointPos(selectedWaypoont.transform, waypointGO.transform);
        var newWaypoint = waypointGO.GetComponent<Waypoint>();

        if (selectedWaypoont.nextWaypoint)
        {
            selectedWaypoont.nextWaypoint.previousWaypoint = newWaypoint;
            newWaypoint.nextWaypoint = selectedWaypoont.nextWaypoint;
        }
        selectedWaypoont.nextWaypoint = newWaypoint;
        newWaypoint.previousWaypoint = selectedWaypoont;
        
        newWaypoint.transform.SetSiblingIndex(selectedWaypoont.transform.GetSiblingIndex() + 1);
        Selection.activeGameObject = waypointGO;
        return newWaypoint;
    }

    private void RemoveWaypoint(Waypoint selectedWaypoont)
    {
        if (selectedWaypoont.nextWaypoint)
        {
            selectedWaypoont.nextWaypoint.previousWaypoint = selectedWaypoont.previousWaypoint;
        }

        if (selectedWaypoont.previousWaypoint)
        {
            if (selectedWaypoont.previousWaypoint.nextWaypoint == selectedWaypoont)
            {
                selectedWaypoont.previousWaypoint.nextWaypoint = selectedWaypoont.nextWaypoint;
                Selection.activeGameObject = selectedWaypoont.previousWaypoint.gameObject;
            }
            else
            {
                var branch = selectedWaypoont.previousWaypoint.branches.
                    First(t => t.waypoint == selectedWaypoont);
                selectedWaypoont.previousWaypoint.branches.Remove(branch);
            }
        }
        
        DestroyImmediate(selectedWaypoont.gameObject);
    }

    private Waypoint CreateBrunch(Waypoint selectedWaypoont)
    {
        var waypointGO = CreateWaypointGo("<BRANCH>");
        var waypoint = waypointGO.GetComponent<Waypoint>();
        selectedWaypoont.branches.Add(new Branch{waypoint = waypoint, cost = 0});
        SetWaypointPos(selectedWaypoont.transform,waypointGO.transform);
        waypoint.previousWaypoint = selectedWaypoont;
        Selection.activeGameObject = waypointGO;
        return waypoint;
    }

    private void CalculateCurrentWaypointBranchesCost(Waypoint selectedWaypoont)
    {
        var planksPerMeter = GameDataKeeper.S.GameSettings.PlanksPerMeter;
        var selectedPos = selectedWaypoont.transform.position;
        for (var i = 0; i < selectedWaypoont.branches.Count; i++)
        {
            var branch = selectedWaypoont.branches[i];
            var distance = (selectedPos - branch.waypoint.transform.position).magnitude;
            branch.cost = Mathf.CeilToInt(distance/planksPerMeter);
            selectedWaypoont.branches[i] = branch;
        }
    }

    private void CalculateAllBranchesCost()
    {
        var waypoints = waypointRoot.GetComponentsInChildren<Waypoint>();
        var waypointsWithBranches = new List<Waypoint>();
        foreach (var waypoint in waypoints)
        {
            if(waypoint.branches.Count > 0)
                waypointsWithBranches.Add(waypoint);
        }

        foreach (var waypoint in waypointsWithBranches)
        {
            CalculateCurrentWaypointBranchesCost(waypoint);
        }
    }

    private void CalculateDistancesToFinish()
    {
        var waypoints = waypointRoot.GetComponentsInChildren<Waypoint>();
        Waypoint lastWaypoint = waypoints[0];
        var minDistance = float.MaxValue;
        var finishPos = FindObjectOfType<Finish>().transform.position;
        foreach (var waypoint in waypoints)
        {
            var sqrDistance = CalculateDistance(waypoint.transform.position, finishPos);
            if (sqrDistance < minDistance)
            {
                lastWaypoint = waypoint;
                minDistance = sqrDistance;
            }
        }
        lastWaypoint.distanceToFinish = minDistance;
        foreach (var waypoint in waypoints)
        {
            CalculateDistanceToFinish(waypoint);
        }
    }

    private float CalculateDistance(Vector3 currentPos, Vector3 targetPos)
    {
        var direction = targetPos - currentPos;
        return Vector3.Magnitude(direction);
    }

    private float CalculateDistanceToFinish(Waypoint waypoint)
    {
        if(waypoint.distanceToFinish > -1f)
            return waypoint.distanceToFinish;
        if (waypoint.nextWaypoint)
        {
            waypoint.distanceToFinish = CalculateDistance(waypoint.transform.position, 
                                            waypoint.nextWaypoint.transform.position) +
                                        CalculateDistanceToFinish(waypoint.nextWaypoint);
            return waypoint.distanceToFinish;
        }
        if (waypoint.branches.Count > 0)
        {
            var closestBranchWaypoint = waypoint.branches[0].waypoint;
            var closestSqrDistance = float.MaxValue;
            foreach (var branch in waypoint.branches)
            {
                var sqrDistance = CalculateDistance(waypoint.transform.position, 
                    branch.waypoint.transform.position);
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestBranchWaypoint = branch.waypoint;
                }
            }
            
            waypoint.distanceToFinish = closestSqrDistance + CalculateDistanceToFinish(closestBranchWaypoint);
            return waypoint.distanceToFinish;
                
        }

        Debug.Log($"Cant calculate distance in {waypoint}");
        return waypoint.distanceToFinish;
    }

    private void GenerateBranches()
    {
        var maxCheckDistance = GameDataKeeper.S.WaypointSettings.MaxBranchCheckDistance;
        var minCheckDistance = GameDataKeeper.S.WaypointSettings.MinBranchCheckDistance;   
        var minCoeff = GameDataKeeper.S.WaypointSettings.MinBranchDistanceCoefficient;

        var waypoints = waypointRoot.GetComponentsInChildren<Waypoint>();

        for (var i = 0; i < waypoints.Length; i++)
        {
            var waypoint = waypoints[i];
            var waypointPos = waypoint.transform.position;
            for (var j = 0; j < waypoints.Length; j++)
            {
                var checkWaypoint = waypoints[j];

                var sqrDistance = CalculateDistance(waypointPos, checkWaypoint.transform.position);
                if(sqrDistance > maxCheckDistance || sqrDistance < minCheckDistance)
                    continue;
                
                var distanceToFinishDifference = waypoint.distanceToFinish - checkWaypoint.distanceToFinish;
                if(distanceToFinishDifference < 0)
                    continue;

                var coeff = distanceToFinishDifference / sqrDistance;

                if (coeff >= minCoeff)
                {
                    waypoint.branches.Add(new Branch(){waypoint = checkWaypoint, coefficient = coeff});
                }
                  
            }
        }
    }

    private void ClearBranches()
    {
        var waypoints = waypointRoot.GetComponentsInChildren<Waypoint>();
        foreach (var waypoint in waypoints)
        {
            if (waypoint.branches.Count > 0)
            {
                waypoint.branches = new List<Branch>();
            }
        }
    }

    private void SortBranches()
    {
        var waypoints = waypointRoot.GetComponentsInChildren<Waypoint>();
        foreach (var waypoint in waypoints)
        {
            if (waypoint.branches.Count > 0)
            {
                waypoint.branches = waypoint.branches.OrderByDescending(branch => branch.coefficient).ToList();
            }
        }
    }

    #endregion

   
    
}
