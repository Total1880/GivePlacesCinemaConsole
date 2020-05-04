using System;
using System.Collections.Generic;
using System.Text;

namespace GivePlacesCinemaConsole
{
    public class FillPlacesOverDifferentRows
    {
        private int _neededPlaces;
        private List<Seat> _usedSeats = new List<Seat>();
        private int _rows;
        private int _columns;
        private string[,] _allPlaces;

        public FillPlacesOverDifferentRows(int neededPlaces, int rows, int columns, string[,] allPlaces)
        {
            _neededPlaces = neededPlaces;
            _rows = rows;
            _columns = columns;
            _allPlaces = allPlaces;

            Go();
        }

        private void Go()
        {
            // zoek eerste vrij plek volgens de regels. Hoever kan hij nog gaan. 
            // Dan naar de columns kijken op de rij + 1. clusters zoeken die gescheiden zijn.
            // Dan naar de columns kijken op de rij - 1. clusters zoeken die gescheiden zijn.
            // Dan naar de columns kijken op de rij + 2. clusters zoeken die gescheiden zijn.
            // enz
            // volledige cluster gevonden? opslagen en naar volgende
            // Slechte clusters ook opslagen zodat er niet onnodig gezocht wordt. 
            // Indien vrije seat in 1 van deze clusters? naar volgende. Altijd deze check doen, ook als je clusters aan het samenstellen bent.
            // Op het einde beste cluster kiezen => zo weinig mogelijk rijen, zo veel mogelijk in het midden (gemiddeld nummer collumns moet zo dicht mogelijk bij middelste liggen. Zelfde bij rij)

        }
    }
}
