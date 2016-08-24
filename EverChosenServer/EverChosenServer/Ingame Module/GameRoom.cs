﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EverChosenServer.Ingame_Module
{
    internal class GameRoom
    {
        /* ---------------------------
         * Manage two players
         * 
         * Activity of players
         * 1. Move unit. (start building, destination building and # of units.)
         * 2. Change building option.
         * 3. Occupy building of enemy of normal.
         * 4. Fight against the enemy.
         * 5. Increment unit count in building automatically.
         * 6. Surrender game.
         * 7. Win or lose game.
        ----------------------------*/

        private Client _player1;
        private Client _player2;
        private List<Building> _mapNodes = new List<Building>();

        /// <summary>
        /// Data for move units from building to building.
        /// </summary>
        private class MoveInfo
        {
            public int StartNode { get; set; }
            public int EndNode { get; set; }
            public int UnitCount { get; set; }
        }

        /// <summary>
        /// Data of one building.
        /// </summary>
        private class Building
        {
            public int Onwer { get; set; }
            public int Kinds { get; set; }
            public int UnitCount { get; set; }
        }


        private class BuildingInfo
        {
            public int Node { get; set; }
            public int Kinds { get; set; }
        }

        public GameRoom(Client a1, Client a2)
        {
            _player1 = a1;
            _player2 = a2;
        }

        public void IngameCommand(object sender, Packet e)
        {
            var client = sender as Client;
            Client target;
            
            if (client == _player1)
                target = _player2;
            else if (client == _player2)
                target = _player1;
            else
            {
                Console.WriteLine("Invalid object.");
                return;
            }

            switch (e.MsgName)
            {
                case "Move":
                    var nodes = JsonConvert.DeserializeObject<MoveInfo>(e.Data);
                    Move(target, nodes.StartNode, nodes.EndNode);
                    //client.BeginSend(e.MsgName, e.Data);
                    //target.BeginSend(e.MsgName, e.Data);
                    break;

                case "Change":
                    var option = JsonConvert.DeserializeObject<BuildingInfo>(e.Data);
                    ChangeUnit(target, option.Node, option.Kinds);
                    //client.BeginSend(e.MsgName, e.Data);
                    //target.BeginSend(e.MsgName, e.Data);
                    break;

                case "Fight":
                    break;

                default:
                    Console.WriteLine(e.MsgName + " is not ingame command.");
                    break;
            }
        }

        private void Move(Client t, int s, int e)
        {
            var movingUnit = 0;

            var packet = new MoveInfo
            {
                StartNode = s,
                EndNode = e,
                UnitCount = movingUnit
            };

            t.BeginSend("Move", packet);
        }

        private void ChangeUnit(Client t, int idx, int kinds)
        {
            //_mapNodes[idx].Kinds = kinds;
            var packet = new BuildingInfo
            {
                Node = idx,
                Kinds = kinds
            };
            
            t.BeginSend("Change", packet);
        }

        private void UseSpell()
        {
            // Need to discuss.
        }

        private void Fight()
        {
            // Need to discuss.
        }

        private void Result()
        {
            
        }
    }
}