﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Core.Operations;
using Newtonsoft.Json;
using EverChosenPacketLib;

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
        private MapInfo _map;

        public GameRoom(Client a1, Client a2, MapInfo map)
        {
            _player1 = a1;
            _player2 = a2;
            _map = map;
            Console.WriteLine("Game room was constructed.");
            Console.WriteLine(_map.MapName);
        }

        /// <summary>
        /// Attached to EventHandler IngameRequest()
        /// </summary>
        /// <param name="sender"> Client who requested. </param>
        /// <param name="e"> Data </param>
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
                case "MapReq":
                    client.BeginSend("MapInfo", _map);
                    break;

                case "Move":
                    var nodes = JsonConvert.DeserializeObject<MoveUnitInfo>(e.Data);
                    var moveInfo = Move(client, nodes.StartNode, nodes.EndNode);

                    client.BeginSend(e.MsgName + "Mine", moveInfo);
                    target.BeginSend(e.MsgName + "Oppo", moveInfo);
                    break;

                case "Change":
                    var option = JsonConvert.DeserializeObject<ChangeBuildingInfo>(e.Data);
                    var buildinfo = ChangeUnit(option.Node, option.Kinds);

                    client.BeginSend(e.MsgName + "Mine", buildinfo);
                    target.BeginSend(e.MsgName + "Oppo", buildinfo);
                    break;

                case "Fight":
                    break;

                default:
                    Console.WriteLine(e.MsgName + " is not ingame command.");
                    break;
            }
        }

        private Unit Move(Client c, int startNode, int endNode)
        {
            var units = _map.MapNodes[startNode].UnitCount /= 2;

            var unitNode = _map.MapNodes[startNode];
            unitNode.UnitCount = units;
            unitNode.Owner = c.MatchingData.TeamColor;

            var info = new Unit
            {
                Units = unitNode,
               // StartNode = startNode,
                EndNode = endNode
            };

            return info;
        }

        private Building ChangeUnit(int idx, int kinds)
        {
            _map.MapNodes[idx].Kinds = kinds;
            _map.MapNodes[idx].UnitCount = 0;

            return _map.MapNodes[idx];
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
