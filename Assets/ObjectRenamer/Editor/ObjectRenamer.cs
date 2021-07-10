#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace OMSB
{
    /// <summary>
    /// 選択オブジェクト名のリネームツール
    /// </summary>
    public class ObjectRenamer : EditorWindow
    {
        //=======================================================================================================
        //. 定数
        //=======================================================================================================
        #region -- 定数

        private const string    TOOL_NAME       = "ObjectRenamer";

        private static readonly string[] TAB_NAMES = new string[] { 
            "追加",
            "置換",
            "連番",
            "削除" 
        };

        #endregion

        //=======================================================================================================
        //. enum
        //=======================================================================================================
        #region -- enum

        private enum ETab {
            Add,
            Replace,
            SerialNum,
            Delete
        }

        private enum EAddPopup {
            Prefix,
            Suffix
        }

        private enum ESerialNumPopup {
            Prefix,
            Suffix
        }

        private enum EDeletePopup {
            Prefix,
            Suffix,
            Select
        }

        #endregion

        //=======================================================================================================
        //. メンバ
        //=======================================================================================================
        #region -- フィールド

        private Vector2         m_BeforeScrollPos       = Vector2.zero;
        private Vector2         m_AfterScrollPos        = Vector2.zero;
        private List<string>    m_AfterObjNames         = new List<string>();
        private ETab            m_SelecteTab            = ETab.Add;

        // Add
        private int             m_AddPosition           = 0;
        private string          m_AddText               = "";
        private EAddPopup       m_AddPopup              = EAddPopup.Prefix;

        // Replace
        private string          m_ReplaceBeforeText     = "";
        private string          m_ReplaceAfterText      = "";

        // SerialNum
        private int             m_SerialNumStart        = 0;
        private int             m_SerialNumDigit        = 1;
        private bool            m_FlagOverride          = false;
        private string          m_OverrideText          = "";
        private ESerialNumPopup m_SerialNumPopup        = ESerialNumPopup.Suffix;

        // Delete
        private int             m_DeleteNumStart        = 0;
        private int             m_DeleteNumDigit        = 1;
        private EDeletePopup    m_DeletePopup           = EDeletePopup.Prefix;

        #endregion

        //=======================================================================================================
        //. イベント
        //=======================================================================================================
        #region -- イベント

        /// <summary>
        /// メニューのWindowに追加
        /// </summary>
        [MenuItem("OMSB/ObjRenamer")]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<ObjectRenamer>(TOOL_NAME);
        }

        /// <summary>
        /// Hierarchyで選択時
        /// </summary>
        void OnSelectionChange()
        {
            //再描画
            Repaint();
        }

        #endregion

        //=======================================================================================================
        //. UI
        //=======================================================================================================
        #region -- UI

        /// <summary>
        /// メイン描画処理
        /// </summary>
        void OnGUI()
        {
            m_SelecteTab = (ETab)GUILayout.Toolbar((int)m_SelecteTab, TAB_NAMES, EditorStyles.toolbarButton);

            switch (m_SelecteTab) {
                case ETab.Add:
                    LayoutAdd();
                    break;
                case ETab.Replace:
                    LayoutReplace();
                    break;
                case ETab.SerialNum:
                    LayoutSerialNum();
                    break;
                case ETab.Delete:
                    LayoutDelete();
                    break;
                default:
                    break;
            }

            LayoutCheck();
        }

        /// <summary>
        /// 追加のレイアウト設定
        /// </summary>
        private void LayoutAdd() 
        {
            if (EditorGUIEx.BeginGroup("Option")) {
                m_AddPopup = (EAddPopup)EditorGUILayout.EnumPopup("前後(Prefix/Suffix)", (System.Enum)m_AddPopup);
                m_AddPosition = EditorGUILayout.IntField("何桁目に", m_AddPosition);
                m_AddText = EditorGUILayout.TextField("追加文字", m_AddText);

                EditorGUILayout.Space();

                if (GUILayout.Button("追加する")) {
                    Rename(AddName());
                }

                EditorGUIEx.EndGroup();
            }
        }

        /// <summary>
        /// 置換のレイアウト設定
        /// </summary>
        private void LayoutReplace() 
        {
            if (EditorGUIEx.BeginGroup("Option")) {
                m_ReplaceBeforeText = EditorGUILayout.TextField("この文字を", m_ReplaceBeforeText);
                m_ReplaceAfterText = EditorGUILayout.TextField("この文字に", m_ReplaceAfterText);

                EditorGUILayout.Space();

                if (GUILayout.Button("置換する")) {
                    Rename(ReplaceName());
                }

                EditorGUIEx.EndGroup();
            }
        }

        /// <summary>
        /// 連番のレイアウト設定
        /// </summary>
        private void LayoutSerialNum() 
        {
            if (EditorGUIEx.BeginGroup("Option")) {
                m_FlagOverride = EditorGUILayout.Toggle("上書き", m_FlagOverride);
                if (m_FlagOverride) {
                    m_OverrideText = EditorGUILayout.TextField("置き換え文字", m_OverrideText);
                }
                m_SerialNumPopup = (ESerialNumPopup)EditorGUILayout.EnumPopup("前後(Prefix/Suffix)", (System.Enum)m_SerialNumPopup);
                m_SerialNumStart = EditorGUILayout.IntField("開始数", m_SerialNumStart);
                m_SerialNumDigit = EditorGUILayout.IntField("桁数", m_SerialNumDigit);

                EditorGUILayout.Space();

                if (GUILayout.Button("連番付け")) {
                    Rename(AddSerialNum());
                }

                EditorGUIEx.EndGroup();
            }
        }

        /// <summary>
        /// 削除のレイアウト設定
        /// </summary>
        private void LayoutDelete() 
        {
            if (EditorGUIEx.BeginGroup("Option")) {
                m_DeletePopup = (EDeletePopup)EditorGUILayout.EnumPopup("削除方法", (System.Enum)m_DeletePopup);
                if (m_DeletePopup == EDeletePopup.Select) {
                    m_DeleteNumStart = EditorGUILayout.IntField("何桁目から", m_DeleteNumStart);
                    m_DeleteNumDigit = EditorGUILayout.IntField("桁数", m_DeleteNumDigit);
                } else {
                    m_DeleteNumDigit = EditorGUILayout.IntField("桁数", m_DeleteNumDigit);
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("削除する")) {
                    Rename(DeleteName());
                }

                EditorGUIEx.EndGroup();
            }
        }

        /// <summary>
        /// 変更確認のレイアウト
        /// </summary>
        private void LayoutCheck() 
        {
            if (EditorGUIEx.BeginGroup("Check")) {
                // 何も選択されていなかったら
                if (!Selection.activeGameObject) {
                    EditorGUILayout.HelpBox("オブジェクトを選択してください", MessageType.Warning);
                } else {
                    using (new EditorGUILayout.HorizontalScope()) {
                        LayoutCheckBefore();
                        LayoutCheckAfter();
                    }
                }
            }
        }

        /// <summary>
        /// 変更前のレイアウト
        /// </summary>
        private void LayoutCheckBefore() 
        {
            using (new EditorGUILayout.VerticalScope()) {
                EditorGUIEx.DrawSubTitle("変更前");
                using (var scroll = new EditorGUILayout.ScrollViewScope(m_BeforeScrollPos, GUI.skin.box)) {
                    m_BeforeScrollPos = scroll.scrollPosition;

                    foreach (var gameObj in GetSelectionGameObjectsSortHierarchyIndex()) {
                        EditorGUILayout.LabelField(gameObj.name);
                    }
                }
            }
        }

        /// <summary>
        /// 変更後のレイアウト
        /// </summary>
        private void LayoutCheckAfter() 
        {
            using (new EditorGUILayout.VerticalScope()) {
                EditorGUIEx.DrawSubTitle("変更後");
                using (var scroll = new EditorGUILayout.ScrollViewScope(m_AfterScrollPos, GUI.skin.box)) {
                    m_AfterScrollPos = scroll.scrollPosition;

                    switch (m_SelecteTab) {
                        case ETab.Add:
                            m_AfterObjNames = AddName();
                            break;
                        case ETab.Replace:
                            m_AfterObjNames = ReplaceName();
                            break;
                        case ETab.SerialNum:
                            m_AfterObjNames = AddSerialNum();
                            break;
                        case ETab.Delete:
                            m_AfterObjNames = DeleteName();
                            break;
                        default:
                            break;
                    }

                    foreach (var objName in m_AfterObjNames) {
                        EditorGUILayout.LabelField(objName);
                    }
                }
            }
        }

        #endregion

        //=======================================================================================================
        //. 設定
        //=======================================================================================================
        #region -- 設定

        /// <summary>
        /// 各リネーム処理の反映処理
        /// </summary>
        /// <param name="_objNames">各リネーム処理後のオブジェクト名のリスト</param>
        private void Rename(List<string> _objNames)
        {
            var selectObjs = new List<GameObject>();
            var count = 0;

            foreach (var obj in Selection.objects) {
                selectObjs.Add(obj as GameObject);
            }

            // Hierarchyの上からの順にソート
            selectObjs.Sort((a, b) => a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex());

            foreach (var obj in selectObjs) {
                var gameObj = obj as GameObject;
                Undo.RecordObject(gameObj, "Rename");
                gameObj.name = _objNames[count];
                count++;
            }
        }

        /// <summary>
        /// 追加の処理
        /// </summary>
        /// <returns>処理後のオブジェクト名のリスト</returns>
        private List<string> AddName()
        {
            var objNames = new List<string>();

            foreach (var gameObj in GetSelectionGameObjectsSortHierarchyIndex()) {
                if (m_AddPopup == EAddPopup.Prefix) {
                    objNames.Add(gameObj.name.Insert(m_AddPosition, m_AddText));
                } else {
                    objNames.Add(gameObj.name.Insert(gameObj.name.Length - m_AddPosition, m_AddText));
                }
            }

            return objNames;
        }

        /// <summary>
        /// 置換の処理
        /// </summary>
        /// <returns>処理後のオブジェクト名のリスト</returns>
        private List<string> ReplaceName()
        {
            var objNames = new List<string>();

            foreach (var gameObj in GetSelectionGameObjectsSortHierarchyIndex()) {
                // 名前に置換前の文字が含まれていない or 置換前の文字列が""だったら
                if (!gameObj.name.Contains(m_ReplaceBeforeText) || m_ReplaceBeforeText == "") {
                    objNames.Add(gameObj.name);
                } else {
                    objNames.Add((gameObj.name.Replace(m_ReplaceBeforeText, m_ReplaceAfterText)));
                }
            }

            return objNames;
        }


        /// <summary>
        /// 連番の処理
        /// </summary>
        /// <returns>処理後のオブジェクト名のリスト</returns>
        private List<string> AddSerialNum()
        {
            var objNames = new List<string>();
            var serialNumCount = m_SerialNumStart;

            foreach (var gameObj in GetSelectionGameObjectsSortHierarchyIndex()) {
                var serialNumName = gameObj.name;

                if (m_FlagOverride) {
                    serialNumName = m_OverrideText;
                }

                if (m_SerialNumPopup == ESerialNumPopup.Prefix) {
                    objNames.Add(serialNumCount.ToString("D" + m_SerialNumDigit) + serialNumName);
                } else {
                    objNames.Add(serialNumName + serialNumCount.ToString("D" + m_SerialNumDigit));
                }

                serialNumCount++;
            }

            return objNames;
        }

        /// <summary>
        /// 名前の文字列を削除する処理
        /// </summary>
        /// <returns>処理後のオブジェクト名のリスト</returns>
        private List<string> DeleteName()
        {
            var objNames = new List<string>();

            foreach (var gameObj in GetSelectionGameObjectsSortHierarchyIndex()) {
                if (m_DeleteNumDigit <= 0 || m_DeleteNumDigit > gameObj.name.Length) {
                    objNames.Add(gameObj.name);
                } else {
                    switch (m_DeletePopup) {
                        case EDeletePopup.Prefix:
                            objNames.Add(gameObj.name.Remove(0, m_DeleteNumDigit));
                            break;
                        case EDeletePopup.Suffix:
                            objNames.Add(gameObj.name.Remove(gameObj.name.Length - m_DeleteNumDigit));
                            break;
                        case EDeletePopup.Select:
                            objNames.Add(gameObj.name.Remove(m_DeleteNumStart, m_DeleteNumDigit));
                            break;
                        default:
                            objNames.Add(gameObj.name);
                            break;
                    }
                }
            }

            return objNames;
        }

        #endregion

        //=======================================================================================================
        //. 取得
        //=======================================================================================================
        #region -- 取得

        /// <summary>
        /// 選択オブジェクトをHierarchyの上から順に取得
        /// </summary>
        /// <returns>Hierarchyの上から順にソートした選択オブジェクト</returns>
        private List<GameObject> GetSelectionGameObjectsSortHierarchyIndex() 
        {
            var selecGameObjs = new List<GameObject>(Selection.gameObjects);

            // Hierarchyの上からの順にソート
            selecGameObjs.Sort((a, b) => a.transform.GetSiblingIndex() - b.transform.GetSiblingIndex());

            return selecGameObjs;
        }

        #endregion
    }
}

#endif