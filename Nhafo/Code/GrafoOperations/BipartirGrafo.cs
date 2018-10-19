using Nhafo.Code.Factories;
using Nhafo.Code.Utils;
using Nhafo.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Nhafo.Code.GrafoOperations {
    public class BipartirGrafo : GrafoColoringOperation {
        public override byte MaxColors => 2;

        public BipartirGrafo(GrafoControl grafo) : base(grafo) { }

        public override GrafoControl Color() {
            if(CanBeDoneResult == null)
                CanBeDone();
            if(CanBeDoneResult == false)
                throw new Exception("Cannot be colored with " + MaxColors + " colors.");

            GrafoControl grafoControl = GrafoFactory.Create();
            //grafoControl.Key = grafo.Key + " - Bipartido";

            double colorAPosition = 0;
            double colorBPosition = 0;
            double colorCPosition = 0;

            VerticeControl[] vertices = new VerticeControl[verticeList.Count];
            VerticeControl verticeControl;
            int i = 0;
            foreach(Vertice v in verticeList) {
                verticeControl =
                    new VerticeControl() {
                        Width = VerticeFactory.VERTICE_DIAMETER,
                        Height = VerticeFactory.VERTICE_DIAMETER,
                        
                        Key = v.Control.Key,
                        Draggable = true
                    };

                grafoControl.AddVertice(verticeControl);
                vertices[i++] = verticeControl;

                if(v.IsClear) {
                    verticeControl.Location = new Point(colorCPosition, VerticeFactory.VERTICE_DIAMETER * 5);
                    verticeControl.Color = Colors.Red;
                    colorCPosition += VerticeFactory.VERTICE_DIAMETER * 2;
                }
                else if(v.ColorId == 0) {
                    verticeControl.Location = new Point(colorAPosition, 0);
                    verticeControl.Color = ColorUtils.AccentColor.Color;
                    colorAPosition += VerticeFactory.VERTICE_DIAMETER * 2;
                }
                else {
                    verticeControl.Location = new Point(colorBPosition, VerticeFactory.VERTICE_DIAMETER * 3);
                    verticeControl.Color = Colors.Gold;
                    colorBPosition += VerticeFactory.VERTICE_DIAMETER * 2;
                }
            }
            
            foreach(ArestaControl a in grafo.Arestas) {
                grafoControl.AddAresta(new ArestaControl() {
                    VerticeA = vertices[FindIndex(a.VerticeA)],
                    VerticeB = vertices[FindIndex(a.VerticeB)]
                });
            }

            GrafoFactory.CenterGrafoContent(grafoControl);
            return grafoControl;
        }
    }
}
//System.Windows.Data Error: 17 : Cannot get 'Background' value (type 'Brush') from '' (type 'Border'). 
//BindingExpression:Path=Background; DataItem='Border' (Name=''); target element is 'Border' (Name=''); target property is 'BorderBrush' (type 'Brush') 
//InvalidOperationException:'System.InvalidOperationException: '#FF119EDA' não é um valor válido para a propriedade 'Background'.