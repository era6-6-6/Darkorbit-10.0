using Darkorbit.Net.netty.handlers;
using Darkorbit.Net.netty.handlers.BattleStationRequestHandlers;
using Darkorbit.Net.netty.handlers.GroupRequestHandlers;
using Darkorbit.Net.netty.handlers.PetRequestHandlers;
using Darkorbit.Net.netty.handlers.UbaRequestHandlers;
using Darkorbit.Net.netty.requests;
using Darkorbit.Net.netty.requests.BattleStationRequests;
using Darkorbit.Net.netty.requests.GroupRequests;

namespace Darkorbit.Net.netty
{
    class Handler
    {
        private static Dictionary<short, IHandler> Commands = new Dictionary<short, IHandler>();

        public static void AddCommands()
        {
            Commands.Add(LegacyModuleRequest.ID, new LegacyModuleHandler());
            Commands.Add(MoveRequest.ID, new MoveRequestHandler());
            Commands.Add(ShipSelectRequest.ID, new ShipSelectRequestHandler());
            Commands.Add(SelectMenuBarItemRequest.ID, new SelectMenuBarItemHandler());
         //   Commands.Add(ReadyRequest.ID, new ReadyRequestHandler());
            Commands.Add(UIOpenRequest.ID, new UIOpenRequestHandler());
            Commands.Add(SlotBarConfigSetRequest.ID, new SlotBarConfigSetRequestHandler());
            Commands.Add(AudioSettingsRequest.ID, new AudioSettingsRequestHandler());
            Commands.Add(DisplaySettingsRequest.ID, new DisplaySettingsRequestHandler());
            Commands.Add(GameplaySettingsRequest.ID, new GameplaySettingsRequestHandler());
            Commands.Add(QualitySettingsRequest.ID, new QualitySettingsRequestHandler());
            Commands.Add(SendWindowUpdateRequest.ID, new SendWindowUpdateRequestHandler());
            Commands.Add(WindowSettingsRequest.ID, new WindowSettingsRequestHandler());
            Commands.Add(UserKeyBindingsUpdateRequest.ID, new UserKeyBindingsUpdateHandler());
            Commands.Add(AssetHandleClickRequest.ID, new AssetHandleClickHandler());
            Commands.Add(KillscreenRequest.ID, new KillsceenRequestHandler());
            Commands.Add(ProActionBarRequest.ID, new ProActionBarRequestHandler());
            Commands.Add(PetRequest.ID, new PetRequestHandler());
            Commands.Add(GroupInvitationRequest.ID, new GroupInvitationRequestHandler());
            Commands.Add(GroupAcceptInvitationRequest.ID, new GroupAcceptInvitationRequestHandler());
            Commands.Add(CollectBoxRequest.ID, new CollectBoxRequestHandler());
            Commands.Add(PetGearActivationRequest.ID, new PetGearActivationRequestHandler());
            Commands.Add(GroupRevokeInvitationRequest.ID, new GroupRevokeInvitationRequestHandler());
            Commands.Add(GroupRejectInvitationRequest.ID, new GroupRejectInvitationRequestHandler());
            Commands.Add(GroupChangeLeaderRequest.ID, new GroupChangeLeaderRequestHandler());
            Commands.Add(GroupPingPlayerRequest.ID, new GroupPingPlayerRequestHandler());
            Commands.Add(GroupPingPositionRequest.ID, new GroupPingPositionRequestHandler());
            Commands.Add(GroupFollowPlayerRequest.ID, new GroupFollowPlayerRequestHandler());
            Commands.Add(GroupKickPlayerRequest.ID, new GroupKickPlayerRequestHandler());

            Commands.Add(EquipModuleRequest.ID, new EquipModuleRequestHandler());
            Commands.Add(BuildStationRequest.ID, new BuildStationRequestHandler());
            Commands.Add(UnEquipModuleRequest.ID, new UnEquipModuleRequestHandler());
            Commands.Add(EmergencyRepairRequest.ID, new EmergencyRepairRequestHandler());

            Commands.Add(2244, new RepairStationRequestHandler());
            Commands.Add(10343, new LogoutCancelRequestHandler());
            Commands.Add(31106, new AttackLaserRequestHandler());
            Commands.Add(1528, new AttackAbortLaserRequestHandler());
            Commands.Add(22801, new AttackRocketRequestHandler());
            Commands.Add(9253, new PortalJumpRequestHandler());
            Commands.Add(11301, new UbaMatchmakingRequestHandler());
            Commands.Add(659, new UbaMatchmakingCancelRequestHandler());
            Commands.Add(65, new UbaMatchmakingAcceptRequestHandler());
            Commands.Add(9103, new GroupLeaveRequestHandler());
            Commands.Add(28685, new GroupChangeGroupBehaviourRequestHandler());
            Commands.Add(26571, new GroupUpdateBlockInvitationStateRequestHandler());
            Commands.Add(ResetRequest.ID, new ResetRequestHandler());

            Commands.Add(QuestGiverRequest.ID, new QuestGiverRequestHandler());
            Commands.Add(QuestGiverCategoryRequest.ID, new QuestGiverCategoryRequestHandler());
            Commands.Add(QuestLoadRequest.ID, new QuestLoadRequestHandler());
        }

        public static void Execute(byte[] bytes, GameClient client)
        {
            try
            {
                var parser = new ByteParser(bytes);

                //Console.WriteLine("id: " + parser.ID+"|"+ DateTime.Now);

                if (parser.ID == LoginRequest.ID)
                {
                    var read = new LoginRequest();
                    read.readCommand(bytes);

                    if (QueryManager.CheckSessionId(read.userID, read.sessionID) && !QueryManager.Banned(read.userID))
                        new LoginRequestHandler(client, read.userID);

                    return;
                }

                var gameSession = GameManager.GetGameSession(client.UserId);
                if (gameSession == null) return;
                if (Commands.ContainsKey(parser.ID))
                {
                    //Console.WriteLine("packet: " + parser.ID);
                    Commands[parser.ID].execute(gameSession, bytes);
                    gameSession.LastActiveTime = DateTime.Now;
                }
                //else Out.WriteLine("Unknown command ID: " + parser.ID);
            }
            catch (Exception e)
            {
                Out.WriteLine("Execute void exception: " + e, "Handler.cs");
                Logger.Log("error_log", $"- [Handler.cs] Execute void exception: {e}");
            }
        }
    }
}
