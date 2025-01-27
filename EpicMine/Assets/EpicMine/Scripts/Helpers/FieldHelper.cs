using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlackTemple.EpicMine.Dto;
using Newtonsoft.Json;
using UnityEngine;

namespace BlackTemple.EpicMine
{
    public static class FieldHelper
    {
        public static List<Core.Field> Fields = new List<Core.Field>();

        public static List<Core.FieldPointArea> PointAreas = new List<Core.FieldPointArea>();
        public static List<Core.FieldFigure> Figures = new List<Core.FieldFigure>();

        public static Core.Field LoadSpecificFieldData(string name)
        {
            var alreadyIn = Fields.Find(x => x.Id == name);

            if (alreadyIn != null)
                return alreadyIn;

            var path = $"{Paths.ResourcesTextDataFieldPath}{name}";

            var data = Resources.Load<TextAsset>(path);

            if (data == null)
                return null;

            var field = JsonConvert.DeserializeObject<Field>(data.text,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});

        
            if (field is FieldFigure figure)
            {
                var coreFigure = new Core.FieldFigure(figure);

                Fields.Add(coreFigure);
                Figures.Add(coreFigure);

                return coreFigure;
            }
            else
            {
                var pointAre = new Core.FieldPointArea(field as FieldPointArea);

                Fields.Add(pointAre);
                PointAreas.Add(pointAre);

                foreach (var pointAreFigure in pointAre.Figures)
                {
                    var exist = Figures.Find(x => x.Id == pointAreFigure.Key);
                    if (exist == null)
                    {
                        LoadSpecificFieldData(pointAreFigure.Key);
                    }
                }

                return pointAre;
            }
        }

        public static Core.FieldPointArea GetDefaultPointArea()
        {
            if (Figures.Count == 0)
                LoadSpecificFieldData(MineLocalConfigs.PointFigure);

            var suiteField = new Core.FieldPointArea();
            suiteField.Default(Figures);
            return suiteField;
        }

        public static Core.FieldPointArea GetSuitePointArea(int tier, int mine, int wall)
        {
            var fieldId = $"{tier}_{mine}_{wall}";

            var suiteField = PointAreas.Find(x => x.Id == fieldId);

            if (suiteField == null)
            {
                fieldId = $"{tier}_{mine}";
                suiteField = PointAreas.Find(x => x.Id == fieldId);
            }
            if (suiteField == null)
            {
                fieldId = $"{tier}";
                suiteField = PointAreas.Find(x => x.Id == fieldId);
            }

            if (suiteField == null)
            {
                suiteField = LoadSpecificFieldData(fieldId) as Core.FieldPointArea;

                if (suiteField == null)
                {
                    if (Figures.Count == 0)
                        LoadSpecificFieldData(MineLocalConfigs.PointFigure);

                    suiteField = new Core.FieldPointArea();
                    suiteField.Default(Figures);
                }
            }

            return suiteField;
        }
    }
}
