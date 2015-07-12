﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProjectK;

namespace EditorK
{
    public class TerrainDetailsPanel : DetailsPanelBase
    {
        public GameObject FillMarkPrefab;

        public Text NameField;
        public Image FillButton;
        public Image EraseButton;
        public Text BrushSizeField;
        public Slider BrushSizeSlider;

        private bool ready = false;
        private bool erasing = false;
        private TerrainFlagInfo info;

        public override void OnEnable()
        {
            base.OnEnable();
            OnFillEraseButtonClick(false);
            OnSizeSliderValueChange();
            ready = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            ready = false;
        }

        public override void Refresh(InfoMap infos)
        {
            base.Refresh(infos);
            int index = (int)infos["index"];

            info = TerrainFlagInfo.Infos[index];
            NameField.text = info.Name;
            SetMouseData();
        }

        public void OnFillEraseButtonClick(bool erase)
        {
            erasing = erase;

            Color white = Color.white;
            Color blue = new Color(0, 0.6f, 1);

            FillButton.color = erasing ? white : blue;
            EraseButton.color = erasing ? blue : white;
            SetMouseData();
        }

        public void OnSizeSliderValueChange()
        {
            BrushSizeField.text = BrushSizeSlider.value.ToString();
            SetMouseData();
        }

        private void SetMouseData()
        {
            if (!ready)
                return;

            InfoMap infos = new InfoMap();
            infos["flag"] = info.Flag;
            infos["size"] = (int)BrushSizeSlider.value;
            infos["erase"] = erasing;

            GameObject preview = Instantiate(FillMarkPrefab) as GameObject;
            preview.GetComponent<SpriteRenderer>().color = info.Color;
            float scale = (BrushSizeSlider.value - 0.5f) * Mathf.Sqrt(3);
            preview.transform.localScale = new Vector3(scale, scale);

            EditorMouse.Instance.Clear();
            EditorMouse.Instance.SetData(EditorMouseDataType.TerrainFill, infos, preview);
        }
    }
}