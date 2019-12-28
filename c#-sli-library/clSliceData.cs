using System;
using System.Drawing;

namespace ThermoBox.slicedata
{
    public class clSliceData
    {
        private struct tyPolygon
        {
            public float[,] coords;
            public bool isHatch;
            public int coords_length;
            
        }

        private System.Collections.Generic.List<tyPolygon> m_polygons = new System.Collections.Generic.List<tyPolygon>();
        
        //-------------------------------------------//
        public clSliceData()
        {
        }

        //-------------------------------------------//
        public void addPolygon(float [,] polygon_coordinates, int pointCount)
        {
            tyPolygon p = new tyPolygon();
            p.coords = polygon_coordinates;
            p.isHatch = false;
            p.coords_length = pointCount;

            m_polygons.Add (p);
        }

        //-------------------------------------------//
        public void addHatch(float[,] polygon_coordinates, int pointCount)
        {
            tyPolygon p = new tyPolygon();
            p.coords = polygon_coordinates;
            p.isHatch = true;
            p.coords_length = pointCount;

            m_polygons.Add (p);
        }

        //-------------------------------------------//
        public bool isHatch(int polygonIndex)
        {
            return m_polygons[polygonIndex].isHatch;
        }

        //-------------------------------------------//
        public int getPolygonCount()
        {
            return m_polygons.Count;
        }

        //-------------------------------------------//
        public int getPointCount(int polygonIndex)
        {
            return m_polygons[polygonIndex].coords_length;
        }

        //-------------------------------------------//
        public float getPointX(int polygonIndex, int PointIndex)
        {
            return m_polygons[polygonIndex].coords[0, PointIndex];
        }

        //-------------------------------------------//
        public float getPointY(int polygonIndex, int PointIndex)
        {
            return m_polygons[polygonIndex].coords[1, PointIndex];
        }

        //-------------------------------------------//
        public float[,] getPolygon(int polygonIndex)
        {
            return m_polygons[polygonIndex].coords;
        }

    

    }
}
