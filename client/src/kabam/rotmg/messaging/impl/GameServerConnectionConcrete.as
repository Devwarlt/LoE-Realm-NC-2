package kabam.rotmg.messaging.impl {
import com.company.assembleegameclient.game.AGameSprite;
import com.company.assembleegameclient.game.events.GuildResultEvent;
import com.company.assembleegameclient.game.events.KeyInfoResponseSignal;
import com.company.assembleegameclient.game.events.NameResultEvent;
import com.company.assembleegameclient.game.events.ReconnectEvent;
import com.company.assembleegameclient.map.AbstractMap;
import com.company.assembleegameclient.map.GroundLibrary;
import com.company.assembleegameclient.map.mapoverlay.CharacterStatusText;
import com.company.assembleegameclient.objects.Container;
import com.company.assembleegameclient.objects.FlashDescription;
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.objects.Merchant;
import com.company.assembleegameclient.objects.NameChanger;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.ObjectProperties;
import com.company.assembleegameclient.objects.Pet;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.objects.Portal;
import com.company.assembleegameclient.objects.Projectile;
import com.company.assembleegameclient.objects.ProjectileProperties;
import com.company.assembleegameclient.objects.SellableObject;
import com.company.assembleegameclient.objects.particles.AOEEffect;
import com.company.assembleegameclient.objects.particles.BurstEffect;
import com.company.assembleegameclient.objects.particles.CollapseEffect;
import com.company.assembleegameclient.objects.particles.ConeBlastEffect;
import com.company.assembleegameclient.objects.particles.FlowEffect;
import com.company.assembleegameclient.objects.particles.HealEffect;
import com.company.assembleegameclient.objects.particles.LightningEffect;
import com.company.assembleegameclient.objects.particles.LineEffect;
import com.company.assembleegameclient.objects.particles.NovaEffect;
import com.company.assembleegameclient.objects.particles.ParticleEffect;
import com.company.assembleegameclient.objects.particles.PoisonEffect;
import com.company.assembleegameclient.objects.particles.RingEffect;
import com.company.assembleegameclient.objects.particles.RisingFuryEffect;
import com.company.assembleegameclient.objects.particles.ShockeeEffect;
import com.company.assembleegameclient.objects.particles.ShockerEffect;
import com.company.assembleegameclient.objects.particles.StreamEffect;
import com.company.assembleegameclient.objects.particles.TeleportEffect;
import com.company.assembleegameclient.objects.particles.ThrowEffect;
import com.company.assembleegameclient.objects.thrown.ThrowProjectileEffect;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.sound.Music;
import com.company.assembleegameclient.sound.SoundEffectLibrary;
import com.company.assembleegameclient.ui.PicView;
import com.company.assembleegameclient.ui.dialogs.Dialog;
import com.company.assembleegameclient.ui.dialogs.NotEnoughFameDialog;
import com.company.assembleegameclient.ui.panels.GuildInvitePanel;
import com.company.assembleegameclient.ui.panels.TradeRequestPanel;
import com.company.assembleegameclient.util.ConditionEffect;
import com.company.assembleegameclient.util.Currency;
import com.company.assembleegameclient.util.FreeList;
import com.company.util.MoreStringUtil;
import com.company.util.Random;
import com.hurlant.crypto.Crypto;
import com.hurlant.crypto.rsa.RSAKey;
import com.hurlant.crypto.symmetric.ICipher;
import com.hurlant.util.Base64;
import com.hurlant.util.der.PEM;

import flash.display.BitmapData;
import flash.events.Event;
import flash.events.TimerEvent;
import flash.geom.Point;
import flash.net.FileReference;
import flash.utils.ByteArray;
import flash.utils.Dictionary;
import flash.utils.Timer;
import flash.utils.getTimer;

import kabam.lib.net.api.MessageMap;
import kabam.lib.net.api.MessageProvider;
import kabam.lib.net.impl.Message;
import kabam.lib.net.impl.SocketServer;
import kabam.rotmg.account.core.Account;
import kabam.rotmg.account.core.view.PurchaseConfirmationDialog;
import kabam.rotmg.arena.control.ArenaDeathSignal;
import kabam.rotmg.arena.control.ImminentArenaWaveSignal;
import kabam.rotmg.arena.model.CurrentArenaRunModel;
import kabam.rotmg.arena.view.BattleSummaryDialog;
import kabam.rotmg.arena.view.ContinueOrQuitDialog;
import kabam.rotmg.chat.model.ChatMessage;
import kabam.rotmg.classes.model.CharacterClass;
import kabam.rotmg.classes.model.CharacterSkin;
import kabam.rotmg.classes.model.CharacterSkinState;
import kabam.rotmg.classes.model.ClassesModel;
import kabam.rotmg.constants.GeneralConstants;
import kabam.rotmg.constants.ItemConstants;
import kabam.rotmg.core.StaticInjectorContext;
import kabam.rotmg.dailyLogin.message.ClaimDailyRewardMessage;
import kabam.rotmg.dailyLogin.message.ClaimDailyRewardResponse;
import kabam.rotmg.dailyLogin.signal.ClaimDailyRewardResponseSignal;
import kabam.rotmg.death.control.HandleDeathSignal;
import kabam.rotmg.death.control.ZombifySignal;
import kabam.rotmg.dialogs.control.CloseDialogsSignal;
import kabam.rotmg.dialogs.control.OpenDialogSignal;
import kabam.rotmg.friends.model.FriendModel;
import kabam.rotmg.game.focus.control.SetGameFocusSignal;
import kabam.rotmg.game.model.GameModel;
import kabam.rotmg.game.model.PotionInventoryModel;
import kabam.rotmg.game.signals.AddSpeechBalloonSignal;
import kabam.rotmg.game.signals.AddTextLineSignal;
import kabam.rotmg.game.signals.GiftStatusUpdateSignal;
import kabam.rotmg.game.view.components.QueuedStatusText;
import kabam.rotmg.maploading.signals.ChangeMapSignal;
import kabam.rotmg.maploading.signals.HideMapLoadingSignal;
import kabam.rotmg.messaging.impl.data.GroundTileData;
import kabam.rotmg.messaging.impl.data.ObjectData;
import kabam.rotmg.messaging.impl.data.ObjectStatusData;
import kabam.rotmg.messaging.impl.data.StatData;
import kabam.rotmg.messaging.impl.incoming.AccountList;
import kabam.rotmg.messaging.impl.incoming.ActivePet;
import kabam.rotmg.messaging.impl.incoming.AllyShoot;
import kabam.rotmg.messaging.impl.incoming.Aoe;
import kabam.rotmg.messaging.impl.incoming.BuyResult;
import kabam.rotmg.messaging.impl.incoming.ClientStat;
import kabam.rotmg.messaging.impl.incoming.CreateSuccess;
import kabam.rotmg.messaging.impl.incoming.Damage;
import kabam.rotmg.messaging.impl.incoming.Death;
import kabam.rotmg.messaging.impl.incoming.EnemyShoot;
import kabam.rotmg.messaging.impl.incoming.EvolvedMessageHandler;
import kabam.rotmg.messaging.impl.incoming.EvolvedPetMessage;
import kabam.rotmg.messaging.impl.incoming.Failure;
import kabam.rotmg.messaging.impl.incoming.File;
import kabam.rotmg.messaging.impl.incoming.GlobalNotification;
import kabam.rotmg.messaging.impl.incoming.Goto;
import kabam.rotmg.messaging.impl.incoming.GuildResult;
import kabam.rotmg.messaging.impl.incoming.InvResult;
import kabam.rotmg.messaging.impl.incoming.InvitedToGuild;
import kabam.rotmg.messaging.impl.incoming.KeyInfoResponse;
import kabam.rotmg.messaging.impl.incoming.MapInfo;
import kabam.rotmg.messaging.impl.incoming.NameResult;
import kabam.rotmg.messaging.impl.incoming.NewAbilityMessage;
import kabam.rotmg.messaging.impl.incoming.NewTick;
import kabam.rotmg.messaging.impl.incoming.Notification;
import kabam.rotmg.messaging.impl.incoming.PasswordPrompt;
import kabam.rotmg.messaging.impl.incoming.PetYard;
import kabam.rotmg.messaging.impl.incoming.Pic;
import kabam.rotmg.messaging.impl.incoming.Ping;
import kabam.rotmg.messaging.impl.incoming.PlaySound;
import kabam.rotmg.messaging.impl.incoming.QuestFetchResponse;
import kabam.rotmg.messaging.impl.incoming.QuestObjId;
import kabam.rotmg.messaging.impl.incoming.QuestRedeemResponse;
import kabam.rotmg.messaging.impl.incoming.QueuePing;
import kabam.rotmg.messaging.impl.incoming.Reconnect;
import kabam.rotmg.messaging.impl.incoming.ReskinUnlock;
import kabam.rotmg.messaging.impl.incoming.ServerFull;
import kabam.rotmg.messaging.impl.incoming.ServerPlayerShoot;
import kabam.rotmg.messaging.impl.incoming.SetFocus;
import kabam.rotmg.messaging.impl.incoming.ShowEffect;
import kabam.rotmg.messaging.impl.incoming.SwitchMusic;
import kabam.rotmg.messaging.impl.incoming.TradeAccepted;
import kabam.rotmg.messaging.impl.incoming.TradeChanged;
import kabam.rotmg.messaging.impl.incoming.TradeDone;
import kabam.rotmg.messaging.impl.incoming.TradeRequested;
import kabam.rotmg.messaging.impl.incoming.TradeStart;
import kabam.rotmg.messaging.impl.incoming.Update;
import kabam.rotmg.messaging.impl.incoming.VerifyEmail;
import kabam.rotmg.messaging.impl.incoming.arena.ArenaDeath;
import kabam.rotmg.messaging.impl.incoming.arena.ImminentArenaWave;
import kabam.rotmg.messaging.impl.incoming.pets.DeletePetMessage;
import kabam.rotmg.messaging.impl.incoming.pets.HatchPetMessage;
import kabam.rotmg.messaging.impl.outgoing.AcceptTrade;
import kabam.rotmg.messaging.impl.outgoing.ActivePetUpdateRequest;
import kabam.rotmg.messaging.impl.outgoing.AoeAck;
import kabam.rotmg.messaging.impl.outgoing.Buy;
import kabam.rotmg.messaging.impl.outgoing.CancelTrade;
import kabam.rotmg.messaging.impl.outgoing.ChangeGuildRank;
import kabam.rotmg.messaging.impl.outgoing.ChangeTrade;
import kabam.rotmg.messaging.impl.outgoing.CheckCredits;
import kabam.rotmg.messaging.impl.outgoing.ChooseName;
import kabam.rotmg.messaging.impl.outgoing.Create;
import kabam.rotmg.messaging.impl.outgoing.CreateGuild;
import kabam.rotmg.messaging.impl.outgoing.EditAccountList;
import kabam.rotmg.messaging.impl.outgoing.EnemyHit;
import kabam.rotmg.messaging.impl.outgoing.EnterArena;
import kabam.rotmg.messaging.impl.outgoing.Escape;
import kabam.rotmg.messaging.impl.outgoing.GoToQuestRoom;
import kabam.rotmg.messaging.impl.outgoing.GotoAck;
import kabam.rotmg.messaging.impl.outgoing.GroundDamage;
import kabam.rotmg.messaging.impl.outgoing.GuildInvite;
import kabam.rotmg.messaging.impl.outgoing.GuildRemove;
import kabam.rotmg.messaging.impl.outgoing.Hello;
import kabam.rotmg.messaging.impl.outgoing.InvDrop;
import kabam.rotmg.messaging.impl.outgoing.InvSwap;
import kabam.rotmg.messaging.impl.outgoing.JoinGuild;
import kabam.rotmg.messaging.impl.outgoing.KeyInfoRequest;
import kabam.rotmg.messaging.impl.outgoing.Load;
import kabam.rotmg.messaging.impl.outgoing.Move;
import kabam.rotmg.messaging.impl.outgoing.OtherHit;
import kabam.rotmg.messaging.impl.outgoing.OutgoingMessage;
import kabam.rotmg.messaging.impl.outgoing.PetUpgradeRequest;
import kabam.rotmg.messaging.impl.outgoing.PlayerHit;
import kabam.rotmg.messaging.impl.outgoing.PlayerShoot;
import kabam.rotmg.messaging.impl.outgoing.PlayerText;
import kabam.rotmg.messaging.impl.outgoing.Pong;
import kabam.rotmg.messaging.impl.outgoing.QuestRedeem;
import kabam.rotmg.messaging.impl.outgoing.QueuePong;
import kabam.rotmg.messaging.impl.outgoing.RequestTrade;
import kabam.rotmg.messaging.impl.outgoing.Reskin;
import kabam.rotmg.messaging.impl.outgoing.ReskinPet;
import kabam.rotmg.messaging.impl.outgoing.SetCondition;
import kabam.rotmg.messaging.impl.outgoing.ShootAck;
import kabam.rotmg.messaging.impl.outgoing.SquareHit;
import kabam.rotmg.messaging.impl.outgoing.Teleport;
import kabam.rotmg.messaging.impl.outgoing.UseItem;
import kabam.rotmg.messaging.impl.outgoing.UsePortal;
import kabam.rotmg.minimap.control.UpdateGameObjectTileSignal;
import kabam.rotmg.minimap.control.UpdateGroundTileSignal;
import kabam.rotmg.minimap.model.UpdateGroundTileVO;
import kabam.rotmg.pets.controller.DeletePetSignal;
import kabam.rotmg.pets.controller.HatchPetSignal;
import kabam.rotmg.pets.controller.NewAbilitySignal;
import kabam.rotmg.pets.controller.PetFeedResultSignal;
import kabam.rotmg.pets.controller.UpdateActivePet;
import kabam.rotmg.pets.controller.UpdatePetYardSignal;
import kabam.rotmg.pets.data.PetsModel;
import kabam.rotmg.questrewards.controller.QuestFetchCompleteSignal;
import kabam.rotmg.questrewards.controller.QuestRedeemCompleteSignal;
import kabam.rotmg.queue.control.ShowQueueSignal;
import kabam.rotmg.queue.control.UpdateQueueSignal;
import kabam.rotmg.servers.api.Server;
import kabam.rotmg.text.model.TextKey;
import kabam.rotmg.text.view.stringBuilder.LineBuilder;
import kabam.rotmg.ui.model.Key;
import kabam.rotmg.ui.model.UpdateGameObjectTileVO;
import kabam.rotmg.ui.signals.ShowHideKeyUISignal;
import kabam.rotmg.ui.signals.ShowKeySignal;
import kabam.rotmg.ui.signals.UpdateBackpackTabSignal;
import kabam.rotmg.ui.view.NotEnoughGoldDialog;
import kabam.rotmg.ui.view.TitleView;

import org.osflash.signals.Signal;
import org.swiftsuspenders.Injector;

import robotlegs.bender.framework.api.ILogger;

public class GameServerConnectionConcrete extends GameServerConnection {

    private static const TO_MILLISECONDS:int = 1000;

    private var petUpdater:PetUpdater;
    private var messages:MessageProvider;
    private var playerId_:int = -1;
    private var player:Player;
    private var retryConnection_:Boolean = true;
    private var rand_:Random = null;
    private var giftChestUpdateSignal:GiftStatusUpdateSignal;
    private var death:Death;
    private var retryTimer_:Timer;
    private var delayBeforeReconnect:int = 1;
    private var addTextLine:AddTextLineSignal;
    private var addSpeechBalloon:AddSpeechBalloonSignal;
    private var updateGroundTileSignal:UpdateGroundTileSignal;
    private var updateGameObjectTileSignal:UpdateGameObjectTileSignal;
    private var logger:ILogger;
    private var handleDeath:HandleDeathSignal;
    private var zombify:ZombifySignal;
    private var setGameFocus:SetGameFocusSignal;
    private var updateBackpackTab:UpdateBackpackTabSignal;
    private var petFeedResult:PetFeedResultSignal;
    private var closeDialogs:CloseDialogsSignal;
    private var openDialog:OpenDialogSignal;
    private var arenaDeath:ArenaDeathSignal;
    private var imminentWave:ImminentArenaWaveSignal;
    private var questFetchComplete:QuestFetchCompleteSignal;
    private var questRedeemComplete:QuestRedeemCompleteSignal;
    private var keyInfoResponse:KeyInfoResponseSignal;
    private var claimDailyRewardResponse:ClaimDailyRewardResponseSignal;
    private var currentArenaRun:CurrentArenaRunModel;
    private var classesModel:ClassesModel;
    private var injector:Injector;
    private var model:GameModel;
    private var updateActivePet:UpdateActivePet;
    private var petsModel:PetsModel;
    private var friendModel:FriendModel;

    public function GameServerConnectionConcrete(_arg1:AGameSprite, _arg2:Server, _arg3:int, _arg4:Boolean, _arg5:int, _arg6:int, _arg7:ByteArray, _arg8:String, _arg9:Boolean) {
        this.injector = StaticInjectorContext.getInjector();
        this.giftChestUpdateSignal = this.injector.getInstance(GiftStatusUpdateSignal);
        this.addTextLine = this.injector.getInstance(AddTextLineSignal);
        this.addSpeechBalloon = this.injector.getInstance(AddSpeechBalloonSignal);
        this.updateGroundTileSignal = this.injector.getInstance(UpdateGroundTileSignal);
        this.updateGameObjectTileSignal = this.injector.getInstance(UpdateGameObjectTileSignal);
        this.petFeedResult = this.injector.getInstance(PetFeedResultSignal);
        this.updateBackpackTab = StaticInjectorContext.getInjector().getInstance(UpdateBackpackTabSignal);
        this.updateActivePet = this.injector.getInstance(UpdateActivePet);
        this.petsModel = this.injector.getInstance(PetsModel);
        this.friendModel = this.injector.getInstance(FriendModel);
        this.closeDialogs = this.injector.getInstance(CloseDialogsSignal);
        changeMapSignal = this.injector.getInstance(ChangeMapSignal);
        this.openDialog = this.injector.getInstance(OpenDialogSignal);
        this.arenaDeath = this.injector.getInstance(ArenaDeathSignal);
        this.imminentWave = this.injector.getInstance(ImminentArenaWaveSignal);
        this.questFetchComplete = this.injector.getInstance(QuestFetchCompleteSignal);
        this.questRedeemComplete = this.injector.getInstance(QuestRedeemCompleteSignal);
        this.keyInfoResponse = this.injector.getInstance(KeyInfoResponseSignal);
        this.claimDailyRewardResponse = this.injector.getInstance(ClaimDailyRewardResponseSignal);
        this.logger = this.injector.getInstance(ILogger);
        this.handleDeath = this.injector.getInstance(HandleDeathSignal);
        this.zombify = this.injector.getInstance(ZombifySignal);
        this.setGameFocus = this.injector.getInstance(SetGameFocusSignal);
        this.classesModel = this.injector.getInstance(ClassesModel);
        serverConnection = this.injector.getInstance(SocketServer);
        this.messages = this.injector.getInstance(MessageProvider);
        this.model = this.injector.getInstance(GameModel);
        this.currentArenaRun = this.injector.getInstance(CurrentArenaRunModel);
        gs_ = _arg1;
        server_ = _arg2;
        gameId_ = _arg3;
        createCharacter_ = _arg4;
        charId_ = _arg5;
        keyTime_ = _arg6;
        key_ = _arg7;
        mapJSON_ = _arg8;
        isFromArena_ = _arg9;
        this.friendModel.setCurrentServer(server_);
        this.getPetUpdater();
        instance = this;
    }

    private static function isStatPotion(_arg1:int):Boolean {
        return ((((((((((((((((((((_arg1 == 2591)) || ((_arg1 == 5465)))) || ((_arg1 == 9064)))) || ((((((_arg1 == 2592)) || ((_arg1 == 5466)))) || ((_arg1 == 9065)))))) || ((((((_arg1 == 2593)) || ((_arg1 == 5467)))) || ((_arg1 == 9066)))))) || ((((((_arg1 == 2612)) || ((_arg1 == 5468)))) || ((_arg1 == 9067)))))) || ((((((_arg1 == 2613)) || ((_arg1 == 5469)))) || ((_arg1 == 9068)))))) || ((((((_arg1 == 2636)) || ((_arg1 == 5470)))) || ((_arg1 == 9069)))))) || ((((((_arg1 == 2793)) || ((_arg1 == 5471)))) || ((_arg1 == 9070)))))) || ((((((_arg1 == 2794)) || ((_arg1 == 5472)))) || ((_arg1 == 9071))))));
    }


    private function getPetUpdater():void {
        this.injector.map(AGameSprite).toValue(gs_);
        this.petUpdater = this.injector.getInstance(PetUpdater);
        this.injector.unmap(AGameSprite);
    }

    override public function disconnect():void {
        this.removeServerConnectionListeners();
        this.unmapMessages();
        serverConnection.disconnect();
    }

    private function removeServerConnectionListeners():void {
        serverConnection.connected.remove(this.onConnected);
        serverConnection.closed.remove(this.onClosed);
        serverConnection.error.remove(this.onError);
    }

    override public function connect():void {
        this.addServerConnectionListeners();
        this.mapMessages();
        var _local1:ChatMessage = new ChatMessage();
        _local1.name = Parameters.CLIENT_CHAT_NAME;
        _local1.text = TextKey.CHAT_CONNECTING_TO;
        var _local2:String = server_.name;
        if (_local2 == '{"text":"server.vault"}') {
            _local2 = "server.vault";
        }
        _local2 = LineBuilder.getLocalizedStringFromKey(_local2);
        _local1.tokens = {"serverName": _local2};
        this.addTextLine.dispatch(_local1);
        serverConnection.connect(server_.address, server_.port);
    }

    public function addServerConnectionListeners():void {
        serverConnection.connected.add(this.onConnected);
        serverConnection.closed.add(this.onClosed);
        serverConnection.error.add(this.onError);
    }

    public function mapMessages():void {
        var _local1:MessageMap = this.injector.getInstance(MessageMap);
        _local1.map(CREATE).toMessage(Create);
        _local1.map(PLAYERSHOOT).toMessage(PlayerShoot);
        _local1.map(MOVE).toMessage(Move);
        _local1.map(PLAYERTEXT).toMessage(PlayerText);
        _local1.map(UPDATEACK).toMessage(Message);
        _local1.map(INVSWAP).toMessage(InvSwap);
        _local1.map(USEITEM).toMessage(UseItem);
        _local1.map(HELLO).toMessage(Hello);
        _local1.map(INVDROP).toMessage(InvDrop);
        _local1.map(PONG).toMessage(Pong);
        _local1.map(LOAD).toMessage(Load);
        _local1.map(SETCONDITION).toMessage(SetCondition);
        _local1.map(TELEPORT).toMessage(Teleport);
        _local1.map(USEPORTAL).toMessage(UsePortal);
        _local1.map(BUY).toMessage(Buy);
        _local1.map(PLAYERHIT).toMessage(PlayerHit);
        _local1.map(ENEMYHIT).toMessage(EnemyHit);
        _local1.map(AOEACK).toMessage(AoeAck);
        _local1.map(SHOOTACK).toMessage(ShootAck);
        _local1.map(OTHERHIT).toMessage(OtherHit);
        _local1.map(SQUAREHIT).toMessage(SquareHit);
        _local1.map(GOTOACK).toMessage(GotoAck);
        _local1.map(GROUNDDAMAGE).toMessage(GroundDamage);
        _local1.map(CHOOSENAME).toMessage(ChooseName);
        _local1.map(CREATEGUILD).toMessage(CreateGuild);
        _local1.map(GUILDREMOVE).toMessage(GuildRemove);
        _local1.map(GUILDINVITE).toMessage(GuildInvite);
        _local1.map(REQUESTTRADE).toMessage(RequestTrade);
        _local1.map(CHANGETRADE).toMessage(ChangeTrade);
        _local1.map(ACCEPTTRADE).toMessage(AcceptTrade);
        _local1.map(CANCELTRADE).toMessage(CancelTrade);
        _local1.map(CHECKCREDITS).toMessage(CheckCredits);
        _local1.map(ESCAPE).toMessage(Escape);
        _local1.map(QUEST_ROOM_MSG).toMessage(GoToQuestRoom);
        _local1.map(JOINGUILD).toMessage(JoinGuild);
        _local1.map(CHANGEGUILDRANK).toMessage(ChangeGuildRank);
        _local1.map(EDITACCOUNTLIST).toMessage(EditAccountList);
        _local1.map(ACTIVE_PET_UPDATE_REQUEST).toMessage(ActivePetUpdateRequest);
        _local1.map(PETUPGRADEREQUEST).toMessage(PetUpgradeRequest);
        _local1.map(ENTER_ARENA).toMessage(EnterArena);
        _local1.map(ACCEPT_ARENA_DEATH).toMessage(OutgoingMessage);
        _local1.map(QUEST_FETCH_ASK).toMessage(OutgoingMessage);
        _local1.map(QUEST_REDEEM).toMessage(QuestRedeem);
        _local1.map(KEY_INFO_REQUEST).toMessage(KeyInfoRequest);
        _local1.map(PET_CHANGE_FORM_MSG).toMessage(ReskinPet);
        _local1.map(CLAIM_LOGIN_REWARD_MSG).toMessage(ClaimDailyRewardMessage);
        _local1.map(FAILURE).toMessage(Failure).toMethod(this.onFailure);
        _local1.map(CREATE_SUCCESS).toMessage(CreateSuccess).toMethod(this.onCreateSuccess);
        _local1.map(SERVERPLAYERSHOOT).toMessage(ServerPlayerShoot).toMethod(this.onServerPlayerShoot);
        _local1.map(DAMAGE).toMessage(Damage).toMethod(this.onDamage);
        _local1.map(UPDATE).toMessage(Update).toMethod(this.onUpdate);
        _local1.map(NOTIFICATION).toMessage(Notification).toMethod(this.onNotification);
        _local1.map(GLOBAL_NOTIFICATION).toMessage(GlobalNotification).toMethod(this.onGlobalNotification);
        _local1.map(NEWTICK).toMessage(NewTick).toMethod(this.onNewTick);
        _local1.map(SHOWEFFECT).toMessage(ShowEffect).toMethod(this.onShowEffect);
        _local1.map(GOTO).toMessage(Goto).toMethod(this.onGoto);
        _local1.map(INVRESULT).toMessage(InvResult).toMethod(this.onInvResult);
        _local1.map(RECONNECT).toMessage(Reconnect).toMethod(this.onReconnect);
        _local1.map(PING).toMessage(Ping).toMethod(this.onPing);
        _local1.map(MAPINFO).toMessage(MapInfo).toMethod(this.onMapInfo);
        _local1.map(PIC).toMessage(Pic).toMethod(this.onPic);
        _local1.map(DEATH).toMessage(Death).toMethod(this.onDeath);
        _local1.map(BUYRESULT).toMessage(BuyResult).toMethod(this.onBuyResult);
        _local1.map(AOE).toMessage(Aoe).toMethod(this.onAoe);
        _local1.map(ACCOUNTLIST).toMessage(AccountList).toMethod(this.onAccountList);
        _local1.map(QUESTOBJID).toMessage(QuestObjId).toMethod(this.onQuestObjId);
        _local1.map(NAMERESULT).toMessage(NameResult).toMethod(this.onNameResult);
        _local1.map(GUILDRESULT).toMessage(GuildResult).toMethod(this.onGuildResult);
        _local1.map(ALLYSHOOT).toMessage(AllyShoot).toMethod(this.onAllyShoot);
        _local1.map(ENEMYSHOOT).toMessage(EnemyShoot).toMethod(this.onEnemyShoot);
        _local1.map(TRADEREQUESTED).toMessage(TradeRequested).toMethod(this.onTradeRequested);
        _local1.map(TRADESTART).toMessage(TradeStart).toMethod(this.onTradeStart);
        _local1.map(TRADECHANGED).toMessage(TradeChanged).toMethod(this.onTradeChanged);
        _local1.map(TRADEDONE).toMessage(TradeDone).toMethod(this.onTradeDone);
        _local1.map(TRADEACCEPTED).toMessage(TradeAccepted).toMethod(this.onTradeAccepted);
        _local1.map(CLIENTSTAT).toMessage(ClientStat).toMethod(this.onClientStat);
        _local1.map(FILE).toMessage(File).toMethod(this.onFile);
        _local1.map(INVITEDTOGUILD).toMessage(InvitedToGuild).toMethod(this.onInvitedToGuild);
        _local1.map(PLAYSOUND).toMessage(PlaySound).toMethod(this.onPlaySound);
        _local1.map(ACTIVEPETUPDATE).toMessage(ActivePet).toMethod(this.onActivePetUpdate);
        _local1.map(NEW_ABILITY).toMessage(NewAbilityMessage).toMethod(this.onNewAbility);
        _local1.map(PETYARDUPDATE).toMessage(PetYard).toMethod(this.onPetYardUpdate);
        _local1.map(EVOLVE_PET).toMessage(EvolvedPetMessage).toMethod(this.onEvolvedPet);
        _local1.map(DELETE_PET).toMessage(DeletePetMessage).toMethod(this.onDeletePet);
        _local1.map(HATCH_PET).toMessage(HatchPetMessage).toMethod(this.onHatchPet);
        _local1.map(IMMINENT_ARENA_WAVE).toMessage(ImminentArenaWave).toMethod(this.onImminentArenaWave);
        _local1.map(ARENA_DEATH).toMessage(ArenaDeath).toMethod(this.onArenaDeath);
        _local1.map(VERIFY_EMAIL).toMessage(VerifyEmail).toMethod(this.onVerifyEmail);
        _local1.map(RESKIN_UNLOCK).toMessage(ReskinUnlock).toMethod(this.onReskinUnlock);
        _local1.map(PASSWORD_PROMPT).toMessage(PasswordPrompt).toMethod(this.onPasswordPrompt);
        _local1.map(QUEST_FETCH_RESPONSE).toMessage(QuestFetchResponse).toMethod(this.onQuestFetchResponse);
        _local1.map(QUEST_REDEEM_RESPONSE).toMessage(QuestRedeemResponse).toMethod(this.onQuestRedeemResponse);
        _local1.map(KEY_INFO_RESPONSE).toMessage(KeyInfoResponse).toMethod(this.onKeyInfoResponse);
        _local1.map(LOGIN_REWARD_MSG).toMessage(ClaimDailyRewardResponse).toMethod(this.onLoginRewardResponse);
        _local1.map(SET_FOCUS).toMessage(SetFocus).toMethod(this.setFocus);
        _local1.map(QUEUE_PONG).toMessage(QueuePong);
        _local1.map(SERVER_FULL).toMessage(ServerFull).toMethod(this.HandleServerFull);
        _local1.map(QUEUE_PING).toMessage(QueuePing).toMethod(this.HandleQueuePing);
        _local1.map(SWITCH_MUSIC).toMessage(SwitchMusic).toMethod(this.onSwitchMusic);
    }

    private function onSwitchMusic(sm:SwitchMusic):void {
        Music.load(sm.music);
    }

    private function HandleServerFull(_arg1:ServerFull):void {
        this.injector.getInstance(ShowQueueSignal).dispatch();
        this.injector.getInstance(UpdateQueueSignal).dispatch(_arg1.position_, _arg1.count_);
    }

    private function HandleQueuePing(_arg1:QueuePing):void {
        this.injector.getInstance(UpdateQueueSignal).dispatch(_arg1.position_, _arg1.count_);
        var qp:QueuePong = (this.messages.require(QUEUE_PONG) as QueuePong);
        qp.serial_ = _arg1.serial_;
        qp.time_ = getTimer();
        serverConnection.queueMessage(qp);
    }

    private function onHatchPet(_arg1:HatchPetMessage):void {
        var _local2:HatchPetSignal = this.injector.getInstance(HatchPetSignal);
        _local2.dispatch(_arg1.petName, _arg1.petSkin);
    }

    private function onDeletePet(_arg1:DeletePetMessage):void {
        var _local2:DeletePetSignal = this.injector.getInstance(DeletePetSignal);
        _local2.dispatch(_arg1.petID);
    }

    private function onNewAbility(_arg1:NewAbilityMessage):void {
        var _local2:NewAbilitySignal = this.injector.getInstance(NewAbilitySignal);
        _local2.dispatch(_arg1.type);
    }

    private function onPetYardUpdate(_arg1:PetYard):void {
        var _local2:UpdatePetYardSignal = StaticInjectorContext.getInjector().getInstance(UpdatePetYardSignal);
        _local2.dispatch(_arg1.type);
    }

    private function onEvolvedPet(_arg1:EvolvedPetMessage):void {
        var _local2:EvolvedMessageHandler = this.injector.getInstance(EvolvedMessageHandler);
        _local2.handleMessage(_arg1);
    }

    private function onActivePetUpdate(_arg1:ActivePet):void {
        this.updateActivePet.dispatch(_arg1.instanceID);
        var _local2:String = (((_arg1.instanceID > 0)) ? this.petsModel.getPet(_arg1.instanceID).getName() : "");
        var _local3:String = (((_arg1.instanceID < 0)) ? TextKey.PET_NOT_FOLLOWING : TextKey.PET_FOLLOWING);
        this.addTextLine.dispatch(ChatMessage.make(Parameters.SERVER_CHAT_NAME, _local3, -1, -1, "", false, {"petName": _local2}));
    }

    private function unmapMessages():void {
        var messageMap:MessageMap = this.injector.getInstance(MessageMap);
        messageMap.unmap(CREATE);
        messageMap.unmap(PLAYERSHOOT);
        messageMap.unmap(MOVE);
        messageMap.unmap(PLAYERTEXT);
        messageMap.unmap(UPDATEACK);
        messageMap.unmap(INVSWAP);
        messageMap.unmap(USEITEM);
        messageMap.unmap(HELLO);
        messageMap.unmap(INVDROP);
        messageMap.unmap(PONG);
        messageMap.unmap(LOAD);
        messageMap.unmap(SETCONDITION);
        messageMap.unmap(TELEPORT);
        messageMap.unmap(USEPORTAL);
        messageMap.unmap(BUY);
        messageMap.unmap(PLAYERHIT);
        messageMap.unmap(ENEMYHIT);
        messageMap.unmap(AOEACK);
        messageMap.unmap(SHOOTACK);
        messageMap.unmap(OTHERHIT);
        messageMap.unmap(SQUAREHIT);
        messageMap.unmap(GOTOACK);
        messageMap.unmap(GROUNDDAMAGE);
        messageMap.unmap(CHOOSENAME);
        messageMap.unmap(CREATEGUILD);
        messageMap.unmap(GUILDREMOVE);
        messageMap.unmap(GUILDINVITE);
        messageMap.unmap(REQUESTTRADE);
        messageMap.unmap(CHANGETRADE);
        messageMap.unmap(ACCEPTTRADE);
        messageMap.unmap(CANCELTRADE);
        messageMap.unmap(CHECKCREDITS);
        messageMap.unmap(ESCAPE);
        messageMap.unmap(QUEST_ROOM_MSG);
        messageMap.unmap(JOINGUILD);
        messageMap.unmap(CHANGEGUILDRANK);
        messageMap.unmap(EDITACCOUNTLIST);
        messageMap.unmap(FAILURE);
        messageMap.unmap(CREATE_SUCCESS);
        messageMap.unmap(SERVERPLAYERSHOOT);
        messageMap.unmap(DAMAGE);
        messageMap.unmap(UPDATE);
        messageMap.unmap(NOTIFICATION);
        messageMap.unmap(GLOBAL_NOTIFICATION);
        messageMap.unmap(NEWTICK);
        messageMap.unmap(SHOWEFFECT);
        messageMap.unmap(GOTO);
        messageMap.unmap(INVRESULT);
        messageMap.unmap(RECONNECT);
        messageMap.unmap(PING);
        messageMap.unmap(MAPINFO);
        messageMap.unmap(PIC);
        messageMap.unmap(DEATH);
        messageMap.unmap(BUYRESULT);
        messageMap.unmap(AOE);
        messageMap.unmap(ACCOUNTLIST);
        messageMap.unmap(QUESTOBJID);
        messageMap.unmap(NAMERESULT);
        messageMap.unmap(GUILDRESULT);
        messageMap.unmap(ALLYSHOOT);
        messageMap.unmap(ENEMYSHOOT);
        messageMap.unmap(TRADEREQUESTED);
        messageMap.unmap(TRADESTART);
        messageMap.unmap(TRADECHANGED);
        messageMap.unmap(TRADEDONE);
        messageMap.unmap(TRADEACCEPTED);
        messageMap.unmap(CLIENTSTAT);
        messageMap.unmap(FILE);
        messageMap.unmap(INVITEDTOGUILD);
        messageMap.unmap(PLAYSOUND);
        messageMap.unmap(SERVER_FULL);
        messageMap.unmap(QUEUE_PING);
        messageMap.unmap(QUEUE_PONG);
        messageMap.unmap(SET_FOCUS);
        messageMap.unmap(SWITCH_MUSIC);
    }

    private function encryptConnection():void {
        var _local1:ICipher;
        var _local2:ICipher;
        if (Parameters.ENABLE_ENCRYPTION) {
            _local1 = Crypto.getCipher("rc4", MoreStringUtil.hexStringToByteArray(Parameters.RC4_INCOMING_CIPHER));
            _local2 = Crypto.getCipher("rc4", MoreStringUtil.hexStringToByteArray(Parameters.RC4_OUTGOING_CIPHER));
            serverConnection.setOutgoingCipher(_local1);
            serverConnection.setIncomingCipher(_local2);
        }
    }

    override public function getNextDamage(_arg1:uint, _arg2:uint):uint {
        return (this.rand_.nextIntRange(_arg1, _arg2));
    }

    override public function enableJitterWatcher():void {
        if (jitterWatcher_ == null) {
            jitterWatcher_ = new JitterWatcher();
        }
    }

    override public function disableJitterWatcher():void {
        if (jitterWatcher_ != null) {
            jitterWatcher_ = null;
        }
    }

    private function create():void {
        var characterClass:CharacterClass = this.classesModel.getSelected();
        var create:Create = (this.messages.require(CREATE) as Create);
        create.classType = characterClass.id;
        create.skinType = characterClass.skins.getSelectedSkin().id;

        serverConnection.queueMessage(create);
    }

    private function load():void {
        var load:Load = (this.messages.require(LOAD) as Load);
        load.charId_ = charId_;
        load.isFromArena_ = isFromArena_;

        serverConnection.queueMessage(load);

        if (isFromArena_) {
            this.openDialog.dispatch(new BattleSummaryDialog());
        }
    }

    override public function playerShoot(projectile:Projectile, attackAmount:int, isDazed:Boolean, isBeserk:Boolean, minAttackFrequency:Number, maxAttackFrequency:Number, weaponRateOfFire:Number, isAbility:Boolean):void {
        var playerShoot:PlayerShoot = (this.messages.require(PLAYERSHOOT) as PlayerShoot);
        playerShoot.bulletId_ = projectile.bulletId_;
        playerShoot.containerType_ = projectile.objectType_;
        playerShoot.startingPos_.x_ = projectile.x_;
        playerShoot.startingPos_.y_ = projectile.y_;
        playerShoot.angle_ = projectile.angle_;
        playerShoot.attackAmount_ = attackAmount;
        playerShoot.isDazed_ = isDazed;
        playerShoot.isBeserk_ = isBeserk;
        playerShoot.minAttackFrequency_ = minAttackFrequency;
        playerShoot.maxAttackFrequency_ = maxAttackFrequency;
        playerShoot.weaponRateOfFire_ = weaponRateOfFire;
        playerShoot.isAbility_ = isAbility;

        serverConnection.queueMessage(playerShoot);
    }

    override public function playerHit(bulletId:int, objectId:int):void {
        var playerHit:PlayerHit = (this.messages.require(PLAYERHIT) as PlayerHit);
        playerHit.bulletId_ = bulletId;
        playerHit.objectId_ = objectId;

        serverConnection.queueMessage(playerHit);
    }

    override public function enemyHit(time:int, bulletId:int, targetId:int, kill:Boolean):void {
        var enemyHit:EnemyHit = (this.messages.require(ENEMYHIT) as EnemyHit);
        enemyHit.time_ = time;
        enemyHit.bulletId_ = bulletId;
        enemyHit.targetId_ = targetId;
        enemyHit.kill_ = kill;

        serverConnection.queueMessage(enemyHit);
    }

    override public function otherHit(time:int, bulletId:int, objectId:int, targetId:int):void {
        var otherHit:OtherHit = (this.messages.require(OTHERHIT) as OtherHit);
        otherHit.time_ = time;
        otherHit.bulletId_ = bulletId;
        otherHit.objectId_ = objectId;
        otherHit.targetId_ = targetId;

        serverConnection.queueMessage(otherHit);
    }

    override public function squareHit(time:int, bulletId:int, objectId:int):void {
        var squareHit:SquareHit = (this.messages.require(SQUAREHIT) as SquareHit);
        squareHit.time_ = time;
        squareHit.bulletId_ = bulletId;
        squareHit.objectId_ = objectId;

        serverConnection.queueMessage(squareHit);
    }

    public function aoeAck(time:int, X:Number, Y:Number):void {
        var aoeAck:AoeAck = (this.messages.require(AOEACK) as AoeAck);
        aoeAck.time_ = time;
        aoeAck.position_.x_ = X;
        aoeAck.position_.y_ = Y;

        serverConnection.queueMessage(aoeAck);
    }

    override public function groundDamage(time:int, X:Number, Y:Number):void {
        var groundDamage:GroundDamage = (this.messages.require(GROUNDDAMAGE) as GroundDamage);
        groundDamage.time_ = time;
        groundDamage.position_.x_ = X;
        groundDamage.position_.y_ = Y;

        serverConnection.queueMessage(groundDamage);
    }

    public function shootAck(time:int):void {
        var shootAck:ShootAck = (this.messages.require(SHOOTACK) as ShootAck);
        shootAck.time_ = time;

        serverConnection.queueMessage(shootAck);
    }

    override public function playerText(text:String):void {
        var playerText:PlayerText = (this.messages.require(PLAYERTEXT) as PlayerText);
        playerText.text_ = text;

        serverConnection.queueMessage(playerText);
    }

    override public function invSwap(player:Player, gameObject1:GameObject, slotId1:int, objectType1:int, gameObject2:GameObject, slotId2:int, objectType2:int):Boolean {
        if (!gs_)
            return false;

        if (player.isTrading) {
            this.addTextLine.dispatch(ChatMessage.make("", "You cannot swap items while trading."));

            SoundEffectLibrary.play("error");

            return false;
        }

        var invSwap:InvSwap = (this.messages.require(INVSWAP) as InvSwap);
        invSwap.time_ = gs_.time;
        invSwap.position_.x_ = player.x_;
        invSwap.position_.y_ = player.y_;
        invSwap.slotObject1_.objectId_ = gameObject1.objectId_;
        invSwap.slotObject1_.slotId_ = slotId1;
        invSwap.slotObject1_.objectType_ = objectType1;
        invSwap.slotObject2_.objectId_ = gameObject2.objectId_;
        invSwap.slotObject2_.slotId_ = slotId2;
        invSwap.slotObject2_.objectType_ = objectType2;

        serverConnection.queueMessage(invSwap);

        var temporarilyInventorySwap:int = gameObject1.equipment_[slotId1];
        gameObject1.equipment_[slotId1] = gameObject2.equipment_[slotId2];
        gameObject2.equipment_[slotId2] = temporarilyInventorySwap;

        SoundEffectLibrary.play("inventory_move_item");

        return true;
    }

    override public function invSwapPotion(player:Player, gameObject1:GameObject, slotId1:int, objectType1:int, gameObject2:GameObject, slotId2:int, objectType2:int):Boolean {
        if (!gs_)
            return (false);

        if (player.isTrading) {
            this.addTextLine.dispatch(ChatMessage.make("", "You cannot swap potions while trading."));

            SoundEffectLibrary.play("error");

            return false;
        }

        var invSwap:InvSwap = (this.messages.require(INVSWAP) as InvSwap);
        invSwap.time_ = gs_.time;
        invSwap.position_.x_ = player.x_;
        invSwap.position_.y_ = player.y_;
        invSwap.slotObject1_.objectId_ = gameObject1.objectId_;
        invSwap.slotObject1_.slotId_ = slotId1;
        invSwap.slotObject1_.objectType_ = objectType1;
        invSwap.slotObject2_.objectId_ = gameObject2.objectId_;
        invSwap.slotObject2_.slotId_ = slotId2;
        invSwap.slotObject2_.objectType_ = objectType2;

        gameObject1.equipment_[slotId1] = ItemConstants.NO_ITEM;

        if (objectType1 == PotionInventoryModel.HEALTH_POTION_ID)
            player.healthPotionCount_++;

        if (objectType1 == PotionInventoryModel.MAGIC_POTION_ID)
            player.magicPotionCount_++;

        serverConnection.queueMessage(invSwap);

        SoundEffectLibrary.play("inventory_move_item");

        return (true);
    }

    override public function invDrop(gameObject:GameObject, slotId:int, objectType:int):void {
        if (gameObject is Player) {
            var player:Player = gameObject as Player;

            if (player.isTrading) {
                this.addTextLine.dispatch(ChatMessage.make("", "You cannot drop items while trading."));

                SoundEffectLibrary.play("error");

                return;
            }
        }

        var invDrop:InvDrop = (this.messages.require(INVDROP) as InvDrop);
        invDrop.slotObject_.objectId_ = gameObject.objectId_;
        invDrop.slotObject_.slotId_ = slotId;
        invDrop.slotObject_.objectType_ = objectType;

        serverConnection.queueMessage(invDrop);

        if (((!((slotId == PotionInventoryModel.HEALTH_POTION_SLOT))) && (!((slotId == PotionInventoryModel.MAGIC_POTION_SLOT)))))
            gameObject.equipment_[slotId] = ItemConstants.NO_ITEM;
    }

    override public function useItem(time:int, objectId:int, slotId:int, objectType:int, X:Number, Y:Number, useType:int, isTrading:Boolean):void {
        if (isTrading) {
            this.addTextLine.dispatch(ChatMessage.make("", "You cannot use items while trading."));

            SoundEffectLibrary.play("error");

            return;
        }

        var useItem:UseItem = (this.messages.require(USEITEM) as UseItem);
        useItem.time_ = time;
        useItem.slotObject_.objectId_ = objectId;
        useItem.slotObject_.slotId_ = slotId;
        useItem.slotObject_.objectType_ = objectType;
        useItem.itemUsePos_.x_ = X;
        useItem.itemUsePos_.y_ = Y;
        useItem.useType_ = useType;

        serverConnection.queueMessage(useItem);
    }

    override public function useItem_new(gameObject:GameObject, slotId:int):Boolean {
        if (gameObject is Player) {
            var player:Player = gameObject as Player;

            if (player.isTrading) {
                this.addTextLine.dispatch(ChatMessage.make("", "You cannot use items while trading."));

                SoundEffectLibrary.play("error");

                return false;
            }
        }

        var xml:XML;
        var itemSlot:int = gameObject.equipment_[slotId];

        if ((((itemSlot >= 0x9000)) && ((itemSlot < 0xF000))))
            xml = ObjectLibrary.xmlLibrary_[36863];
        else
            xml = ObjectLibrary.xmlLibrary_[itemSlot];

        if (((((xml) && (!(gameObject.isPaused())))) && (((xml.hasOwnProperty("Consumable")) || (xml.hasOwnProperty("InvUse")))))) {
            if (!this.validStatInc(itemSlot, gameObject)) {
                this.addTextLine.dispatch(ChatMessage.make("", (xml.attribute("id") + " not consumed. Already at Max.")));
                return false;
            }

            if (isStatPotion(itemSlot))
                this.addTextLine.dispatch(ChatMessage.make("", (xml.attribute("id") + " Consumed ++")));

            this.applyUseItem(gameObject, slotId, itemSlot, xml);

            SoundEffectLibrary.play("use_potion");

            return true;
        }

        SoundEffectLibrary.play("error");

        return false;
    }

    private function validStatInc(itemId:int, itemOwner:GameObject):Boolean {
        var p:Player;
        try {
            if ((itemOwner is Player)) {
                p = (itemOwner as Player);
            }
            else {
                p = this.player;
            }
            if ((((((((((((((((((((((itemId == 2591)) || ((itemId == 5465)))) || ((itemId == 9064)))) && ((p.attackMax_ == (Parameters.parse(p.attack_) - Parameters.parse(p.attackBoost_)))))) || ((((((((itemId == 2592)) || ((itemId == 5466)))) || ((itemId == 9065)))) && ((p.defenseMax_ == (Parameters.parse(p.defense_) - Parameters.parse(p.defenseBoost_)))))))) || ((((((((itemId == 2593)) || ((itemId == 5467)))) || ((itemId == 9066)))) && ((p.speedMax_ == (Parameters.parse(p.speed_) - Parameters.parse(p.speedBoost_)))))))) || ((((((((itemId == 2612)) || ((itemId == 5468)))) || ((itemId == 9067)))) && ((p.vitalityMax_ == (Parameters.parse(p.vitality_) - Parameters.parse(p.vitalityBoost_)))))))) || ((((((((itemId == 2613)) || ((itemId == 5469)))) || ((itemId == 9068)))) && ((p.wisdomMax_ == (Parameters.parse(p.wisdom_) - Parameters.parse(p.wisdomBoost_)))))))) || ((((((((itemId == 2636)) || ((itemId == 5470)))) || ((itemId == 9069)))) && ((p.dexterityMax_ == (Parameters.parse(p.dexterity_) - Parameters.parse(p.dexterityBoost_)))))))) || ((((((((itemId == 2793)) || ((itemId == 5471)))) || ((itemId == 9070)))) && ((p.maxHPMax_ == (p.maxHP_ - Parameters.parse(p.maxHPBoost_)))))))) || ((((((((itemId == 2794)) || ((itemId == 5472)))) || ((itemId == 9071)))) && ((p.maxMPMax_ == (p.maxMP_ - Parameters.parse(p.maxMPBoost_)))))))) {
                return (false);
            }
        }
        catch (err:Error) {
            logger.error(("PROBLEM IN STAT INC " + err.getStackTrace()));
        }
        return (true);
    }

    private function applyUseItem(gameObject:GameObject, slotId:int, objectType:int, xml:XML):void {
        var useItem:UseItem = (this.messages.require(USEITEM) as UseItem);
        useItem.time_ = getTimer();
        useItem.slotObject_.objectId_ = gameObject.objectId_;
        useItem.slotObject_.slotId_ = slotId;
        useItem.slotObject_.objectType_ = objectType;
        useItem.itemUsePos_.x_ = 0;
        useItem.itemUsePos_.y_ = 0;

        serverConnection.queueMessage(useItem);

        if (xml.hasOwnProperty("Consumable")) {
            gameObject.equipment_[slotId] = -1;

            if (((xml.hasOwnProperty("Activate")) && ((xml.Activate == "UnlockSkin")))) { }
        }
    }

    override public function setCondition(conditionEffect:uint, conditionDuration:Number):void {
        var setCondition:SetCondition = (this.messages.require(SETCONDITION) as SetCondition);
        setCondition.conditionEffect_ = conditionEffect;
        setCondition.conditionDuration_ = conditionDuration;

        serverConnection.queueMessage(setCondition);
    }

    public function move(player:Player):void {
        if (player == null)
            return;

        if (!player || player.isPaused())
            return;

        var move:Move = (this.messages.require(MOVE) as Move);
        move.position_.x_ = player.x_;
        move.position_.y_ = player.y_;

        serverConnection.queueMessage(move);

        (player && player.onMove());
    }

    override public function teleport(objectId:int):void {
        var teleport:Teleport = (this.messages.require(TELEPORT) as Teleport);
        teleport.objectId_ = objectId;

        serverConnection.queueMessage(teleport);
    }

    override public function usePortal(objectId:int):void {
        var usePortal:UsePortal = (this.messages.require(USEPORTAL) as UsePortal);
        usePortal.objectId_ = objectId;

        serverConnection.queueMessage(usePortal);

        this.checkDavyKeyRemoval();
    }

    private function checkDavyKeyRemoval():void {
        if (gs_.map && gs_.map.name_ == "Davy Jones' Locker")
            ShowHideKeyUISignal.instance.dispatch();
    }

    override public function buy(sellableObjectId:int, quantity:int):void {
        var sObj:SellableObject;
        var converted:Boolean;
        if (outstandingBuy_)
            return;

        sObj = gs_.map.goDict_[sellableObjectId];

        if (sObj == null)
            return;

        converted = false;

        if (sObj.currency_ == Currency.GOLD)
            converted = ((((gs_.model.getConverted()) || ((this.player.credits_ > 100)))) || ((sObj.price_ > this.player.credits_)));

        if (sObj.soldObjectName() == TextKey.VAULT_CHEST)
            this.openDialog.dispatch(new PurchaseConfirmationDialog(function ():void {
                buyConfirmation(sellableObjectId, quantity);
            }));
        else
            this.buyConfirmation(sellableObjectId, quantity);
    }

    private function buyConfirmation(objectId:int, quantity:int):void {
        outstandingBuy_ = true;

        var buy:Buy = (this.messages.require(BUY) as Buy);
        buy.objectId_ = objectId;
        buy.quantity_ = quantity;

        serverConnection.queueMessage(buy);
    }

    public function gotoAck(time:int):void {
        var gotoAck:GotoAck = (this.messages.require(GOTOACK) as GotoAck);
        gotoAck.time_ = time;

        serverConnection.queueMessage(gotoAck);
    }

    override public function editAccountList(accountListId:int, add:Boolean, objectId:int):void {
        var editAccountList:EditAccountList = (this.messages.require(EDITACCOUNTLIST) as EditAccountList);
        editAccountList.accountListId_ = accountListId;
        editAccountList.add_ = add;
        editAccountList.objectId_ = objectId;

        serverConnection.queueMessage(editAccountList);
    }

    override public function chooseName(name:String):void {
        var chooseName:ChooseName = (this.messages.require(CHOOSENAME) as ChooseName);
        chooseName.name_ = name;

        serverConnection.queueMessage(chooseName);
    }

    override public function createGuild(name:String):void {
        var createGuild:CreateGuild = (this.messages.require(CREATEGUILD) as CreateGuild);
        createGuild.name_ = name;

        serverConnection.queueMessage(createGuild);
    }

    override public function guildRemove(_arg1:String):void {
        var _local2:GuildRemove = (this.messages.require(GUILDREMOVE) as GuildRemove);
        _local2.name_ = _arg1;
        serverConnection.queueMessage(_local2);
    }

    override public function guildInvite(name:String):void {
        var guildInvite:GuildInvite = (this.messages.require(GUILDINVITE) as GuildInvite);
        guildInvite.name_ = name;

        serverConnection.queueMessage(guildInvite);
    }

    override public function requestTrade(name:String):void {
        var requestTrade:RequestTrade = (this.messages.require(REQUESTTRADE) as RequestTrade);
        requestTrade.name_ = name;

        serverConnection.queueMessage(requestTrade);
    }

    override public function changeTrade(offer:Vector.<Boolean>):void {
        var changeTrade:ChangeTrade = (this.messages.require(CHANGETRADE) as ChangeTrade);
        changeTrade.offer_ = offer;

        serverConnection.queueMessage(changeTrade);
    }

    override public function acceptTrade(myOffer:Vector.<Boolean>, yourOffer:Vector.<Boolean>):void {
        var acceptTrade:AcceptTrade = (this.messages.require(ACCEPTTRADE) as AcceptTrade);
        acceptTrade.myOffer_ = myOffer;
        acceptTrade.yourOffer_ = yourOffer;

        serverConnection.queueMessage(acceptTrade);
    }

    override public function cancelTrade():void {
        serverConnection.queueMessage(this.messages.require(CANCELTRADE));
    }

    override public function checkCredits():void {
        serverConnection.queueMessage(this.messages.require(CHECKCREDITS));
    }

    override public function escape():void {
        if (this.playerId_ == -1)
            return;

        if (gameId_ == -2) {
            var addTextLineSignal:AddTextLineSignal = StaticInjectorContext.getInjector().getInstance(AddTextLineSignal);
            var chatMessage:ChatMessage = new ChatMessage();
            chatMessage.name = Parameters.SERVER_CHAT_NAME;
            chatMessage.text = "server.already_nexus";

            addTextLineSignal.dispatch(chatMessage);

            return;
        }

        if (gs_.map && gs_.map.name_ == "Arena") {
            serverConnection.sendMessage(this.messages.require(ACCEPT_ARENA_DEATH));

            return;
        }

        this.checkDavyKeyRemoval();

        reconnect2Nexus();
    }

    private function reconnect2Nexus():void {
        var server:Server = new Server();
        server.setName("Nexus");
        server.setAddress(server_.address);
        server.setPort(server_.port);

        var reconnectEvent:ReconnectEvent = new ReconnectEvent(server, -2, false, charId_, 0, null, isFromArena_);
        gs_.dispatchEvent(reconnectEvent);
    }

    override public function gotoQuestRoom():void {
        serverConnection.queueMessage(this.messages.require(QUEST_ROOM_MSG));
    }

    override public function joinGuild(guildName:String):void {
        var joinGuild:JoinGuild = (this.messages.require(JOINGUILD) as JoinGuild);
        joinGuild.guildName_ = guildName;

        serverConnection.queueMessage(joinGuild);
    }

    override public function changeGuildRank(name:String, guildRank:int):void {
        var changeGuildRank:ChangeGuildRank = (this.messages.require(CHANGEGUILDRANK) as ChangeGuildRank);
        changeGuildRank.name_ = name;
        changeGuildRank.guildRank_ = guildRank;

        serverConnection.queueMessage(changeGuildRank);
    }

    private function rsaEncrypt(_arg1:String):String {
        var rsaKey:RSAKey = PEM.readRSAPublicKey(Parameters.RSA_PUBLIC_KEY);
        var byteArray:ByteArray = new ByteArray();
        byteArray.writeUTFBytes(_arg1);

        var byteArray2:ByteArray = new ByteArray();
        rsaKey.encrypt(byteArray, byteArray2, byteArray.length);

        return Base64.encodeByteArray(byteArray2);
    }

    private function onConnected():void {
        var account:Account = StaticInjectorContext.getInjector().getInstance(Account);

        this.addTextLine.dispatch(ChatMessage.make(Parameters.CLIENT_CHAT_NAME, "You have been connected to the server!"));

        this.encryptConnection();

        var hello:Hello = (this.messages.require(HELLO) as Hello);
        hello.buildVersion_ = Parameters.FULL_BUILD;
        hello.gameId_ = gameId_;
        hello.guid_ = this.rsaEncrypt(account.getUserId());
        hello.password_ = this.rsaEncrypt(account.getPassword());
        hello.secret_ = this.rsaEncrypt(account.getSecret());
        hello.keyTime_ = keyTime_;
        hello.key_.length = 0;

        ((!((key_ == null))) && (hello.key_.writeBytes(key_)));

        hello.mapJSON_ = (((mapJSON_ == null)) ? "" : mapJSON_);
        hello.entrytag_ = account.getEntryTag();
        hello.gameNet = account.gameNetwork();
        hello.gameNetUserId = account.gameNetworkUserId();
        hello.playPlatform = account.playPlatform();
        hello.platformToken = account.getPlatformToken();

        serverConnection.sendMessage(hello);
    }

    private function onCreateSuccess(_arg1:CreateSuccess):void {
        this.playerId_ = _arg1.objectId_;
        charId_ = _arg1.charId_;
        gs_.initialize();
        createCharacter_ = false;
    }

    private function onDamage(_arg1:Damage):void {
        var _local5:int;
        var _local2:AbstractMap = gs_.map;
        var _local3:Projectile;
        if ((((_arg1.objectId_ >= 0)) && ((_arg1.bulletId_ > 0)))) {
            _local5 = Projectile.findObjId(_arg1.objectId_, _arg1.bulletId_);
            _local3 = (_local2.boDict_[_local5] as Projectile);
            if (((!((_local3 == null))) && (!(_local3.projProps_.multiHit_)))) {
                _local2.removeObj(_local5);
            }
        }
        var _local4:GameObject = _local2.goDict_[_arg1.targetId_];
        if (_local4 != null) {
            _local4.damage(-1, _arg1.damageAmount_, _arg1.effects_, _arg1.kill_, _local3);
        }
    }

    private function onServerPlayerShoot(_arg1:ServerPlayerShoot):void {
        var _local2:Boolean = (_arg1.ownerId_ == this.playerId_);
        var _local3:GameObject = gs_.map.goDict_[_arg1.ownerId_];
        if (_local3 == null) {
            if (_local2) {
                this.shootAck(-1);
            }
            return;
        }
        if (((!((_local3.objectId_ == this.playerId_))) && (Parameters.data_.disableAllyParticles))) {
            return;
        }
        var _local4:Projectile = (FreeList.newObject(Projectile) as Projectile);
        var _local5:Player = (_local3 as Player);
        if (_local5 != null) {
            _local4.reset(_arg1.containerType_, 0, _arg1.ownerId_, _arg1.bulletId_, _arg1.angle_, gs_.time, _local5.projectileIdSetOverrideNew, _local5.projectileIdSetOverrideOld);
        }
        else {
            _local4.reset(_arg1.containerType_, 0, _arg1.ownerId_, _arg1.bulletId_, _arg1.angle_, gs_.time);
        }
        _local4.setDamage(_arg1.damage_);
        gs_.map.addObj(_local4, _arg1.startingPos_.x_, _arg1.startingPos_.y_);
        if (_local2) {
            this.shootAck(gs_.time);
        }
    }

    private function onAllyShoot(_arg1:AllyShoot):void {
        var _local2:GameObject = gs_.map.goDict_[_arg1.ownerId_];
        if ((((((_local2 == null)) || (_local2.dead_))) || (Parameters.data_.disableAllyParticles))) {
            return;
        }
        var _local3:Projectile = (FreeList.newObject(Projectile) as Projectile);
        var _local4:Player = (_local2 as Player);
        if (_local4 != null) {
            _local3.reset(_arg1.containerType_, 0, _arg1.ownerId_, _arg1.bulletId_, _arg1.angle_, gs_.time, _local4.projectileIdSetOverrideNew, _local4.projectileIdSetOverrideOld);
        }
        else {
            _local3.reset(_arg1.containerType_, 0, _arg1.ownerId_, _arg1.bulletId_, _arg1.angle_, gs_.time);
        }
        gs_.map.addObj(_local3, _local2.x_, _local2.y_);
        _local2.setAttack(_arg1.containerType_, _arg1.angle_);
    }

    private function onReskinUnlock(_arg1:ReskinUnlock):void {
        var _local2:String;
        var _local3:CharacterSkin;
        for (_local2 in this.model.player.lockedSlot) {
            if (this.model.player.lockedSlot[_local2] == _arg1.skinID) {
                this.model.player.lockedSlot[_local2] = 0;
            }
        }
        _local3 = this.classesModel.getCharacterClass(this.model.player.objectType_).skins.getSkin(_arg1.skinID);
        _local3.setState(CharacterSkinState.OWNED);
    }

    private function onEnemyShoot(_arg1:EnemyShoot):void {
        var _local4:Projectile;
        var _local5:Number;
        var _local2:GameObject = gs_.map.goDict_[_arg1.ownerId_];
        if (_local2 == null) {
            this.shootAck(-1);
            return;
        }
        var _local3:int;
        while (_local3 < _arg1.numShots_) {
            _local4 = (FreeList.newObject(Projectile) as Projectile);
            _local5 = (_arg1.angle_ + (_arg1.angleInc_ * _local3));
            _local4.reset(_local2.objectType_, _arg1.bulletType_, _arg1.ownerId_, (_arg1.bulletId_ + _local3) % 0x0100, _local5, gs_.time);
            _local4.setDamage(_arg1.damage_);
            gs_.map.addObj(_local4, _arg1.startingPos_.x_, _arg1.startingPos_.y_);
            _local3++;
        }
        this.shootAck(gs_.time);
        _local2.setAttack(_local2.objectType_, (_arg1.angle_ + (_arg1.angleInc_ * ((_arg1.numShots_ - 1) / 2))));
    }

    private function onTradeRequested(_arg1:TradeRequested):void {
        if (!Parameters.data_.chatTrade) {
            return;
        }
        if (((Parameters.data_.tradeWithFriends) && (!(this.friendModel.isMyFriend(_arg1.name_))))) {
            return;
        }
        if (Parameters.data_.showTradePopup) {
            gs_.hudView.interactPanel.setOverride(new TradeRequestPanel(gs_, _arg1.name_));
        }
        this.addTextLine.dispatch(ChatMessage.make("", ((((_arg1.name_ + " wants to ") + 'trade with you.  Type "/trade ') + _arg1.name_) + '" to trade.')));
    }

    private function onTradeStart(_arg1:TradeStart):void {
        gs_.hudView.startTrade(gs_, _arg1);
    }

    private function onTradeChanged(_arg1:TradeChanged):void {
        gs_.hudView.tradeChanged(_arg1);
    }

    private function onTradeDone(_arg1:TradeDone):void {
        var _local3:Object;
        var _local4:Object;
        gs_.hudView.tradeDone();
        var _local2:String = "";
        try {
            _local4 = JSON.parse(_arg1.description_);
            _local2 = _local4.key;
            _local3 = _local4.tokens;
        }
        catch (e:Error) {
        }
        this.addTextLine.dispatch(ChatMessage.make(Parameters.SERVER_CHAT_NAME, _local2, -1, -1, "", false, _local3));
    }

    private function onTradeAccepted(_arg1:TradeAccepted):void {
        gs_.hudView.tradeAccepted(_arg1);
    }

    private function addObject(_arg1:ObjectData):void {
        var _local2:AbstractMap = gs_.map;
        var _local3:GameObject = ObjectLibrary.getObjectFromType(_arg1.objectType_);
        if (_local3 == null) {
            return;
        }
        var _local4:ObjectStatusData = _arg1.status_;
        _local3.setObjectId(_local4.objectId_);
        _local2.addObj(_local3, _local4.pos_.x_, _local4.pos_.y_);
        if ((_local3 is Player)) {
            this.handleNewPlayer((_local3 as Player), _local2);
        }
        this.processObjectStatus(_local4, 0, -1);
        if (((((_local3.props_.static_) && (_local3.props_.occupySquare_))) && (!(_local3.props_.noMiniMap_)))) {
            this.updateGameObjectTileSignal.dispatch(new UpdateGameObjectTileVO(_local3.x_, _local3.y_, _local3));
        }
    }

    private function handleNewPlayer(_arg1:Player, _arg2:AbstractMap):void {
        this.setPlayerSkinTemplate(_arg1, 0);
        if (_arg1.objectId_ == this.playerId_) {
            this.player = _arg1;
            this.model.player = _arg1;
            _arg2.player_ = _arg1;
            gs_.setFocus(_arg1);
            this.setGameFocus.dispatch(this.playerId_.toString());
        }
    }

    private function onUpdate(update:Update):void {
        var updateCounter:int;
        var groundTileData:GroundTileData;
        var message:Message = this.messages.require(UPDATEACK);

        serverConnection.queueMessage(message);

        updateCounter = 0;

        while (updateCounter < update.tiles_.length) {
            groundTileData = update.tiles_[updateCounter];
            gs_.map.setGroundTile(groundTileData.x_, groundTileData.y_, groundTileData.type_);
            this.updateGroundTileSignal.dispatch(new UpdateGroundTileVO(groundTileData.x_, groundTileData.y_, groundTileData.type_));
            updateCounter++;
        }

        updateCounter = 0;

        while (updateCounter < update.newObjs_.length) {
            this.addObject(update.newObjs_[updateCounter]);
            updateCounter++;
        }

        updateCounter = 0;

        while (updateCounter < update.drops_.length) {
            gs_.map.removeObj(update.drops_[updateCounter]);
            updateCounter++;
        }
    }

    private function onNotification(notification:Notification):void {
        var lineBuilder:LineBuilder;
        var characterStatusText:CharacterStatusText;
        var queuedStatusText:QueuedStatusText;
        var gameObject:GameObject = gs_.map.goDict_[notification.objectId_];

        if (gameObject != null) {
            lineBuilder = LineBuilder.fromJSON(notification.message);

            if (lineBuilder.key == "server.plus_symbol") {
                characterStatusText = new CharacterStatusText(gameObject, notification.color_, 1000);
                characterStatusText.setStringBuilder(lineBuilder);
                gs_.map.mapOverlay_.addStatusText(characterStatusText);
            }
            else {
                queuedStatusText = new QueuedStatusText(gameObject, lineBuilder, notification.color_, 2000);
                gs_.map.mapOverlay_.addQueuedText(queuedStatusText);

                if ((((gameObject == this.player)) && ((lineBuilder.key == "server.quest_complete"))))
                    gs_.map.quest_.completed();
            }
        }
    }

    private function onGlobalNotification(_arg1:GlobalNotification):void {
        switch (_arg1.text) {
            case "yellow":
                ShowKeySignal.instance.dispatch(Key.YELLOW);
                return;
            case "red":
                ShowKeySignal.instance.dispatch(Key.RED);
                return;
            case "green":
                ShowKeySignal.instance.dispatch(Key.GREEN);
                return;
            case "purple":
                ShowKeySignal.instance.dispatch(Key.PURPLE);
                return;
            case "showKeyUI":
                ShowHideKeyUISignal.instance.dispatch();
                return;
            case "giftChestOccupied":
                this.giftChestUpdateSignal.dispatch(GiftStatusUpdateSignal.HAS_GIFT);
                return;
            case "giftChestEmpty":
                this.giftChestUpdateSignal.dispatch(GiftStatusUpdateSignal.HAS_NO_GIFT);
                return;
            case "beginnersPackage":
                return;
        }
    }

    private function onNewTick(_arg1:NewTick):void {
        var _local2:ObjectStatusData;
        if (jitterWatcher_ != null) {
            jitterWatcher_.record();
        }
        this.move(this.player);
        for each (_local2 in _arg1.statuses_) {
            this.processObjectStatus(_local2, _arg1.tickTime_, _arg1.tickId_);
        }
        lastTickId_ = _arg1.tickId_;
    }

    private function canShowEffect(_arg1:GameObject):Boolean {
        if (_arg1 != null) {
            return (true);
        }
        var _local2:Boolean = (_arg1.objectId_ == this.playerId_);
        if (((((!(_local2)) && (_arg1.props_.isPlayer_))) && (Parameters.data_.disableAllyParticles))) {
            return (false);
        }
        return (true);
    }

    private function onShowEffect(_arg1:ShowEffect):void {
        var _local3:GameObject;
        var _local4:ParticleEffect;
        var _local5:Point;
        var _local6:uint;
        var _local2:AbstractMap = gs_.map;
        switch (_arg1.effectType_) {
            case ShowEffect.HEAL_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local2.addObj(new HealEffect(_local3, _arg1.color_), _local3.x_, _local3.y_);
                return;
            case ShowEffect.TELEPORT_EFFECT_TYPE:
                _local2.addObj(new TeleportEffect(), _arg1.pos1_.x_, _arg1.pos1_.y_);
                return;
            case ShowEffect.STREAM_EFFECT_TYPE:
                _local4 = new StreamEffect(_arg1.pos1_, _arg1.pos2_, _arg1.color_);
                _local2.addObj(_local4, _arg1.pos1_.x_, _arg1.pos1_.y_);
                return;
            case ShowEffect.THROW_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                _local5 = (((_local3) != null) ? new Point(_local3.x_, _local3.y_) : _arg1.pos2_.toPoint());
                if (((!((_local3 == null))) && (!(this.canShowEffect(_local3))))) break;
                _local4 = new ThrowEffect(_local5, _arg1.pos1_.toPoint(), _arg1.color_);
                _local2.addObj(_local4, _local5.x, _local5.y);
                return;
            case ShowEffect.NOVA_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new NovaEffect(_local3, _arg1.pos1_.x_, _arg1.color_);
                _local2.addObj(_local4, _local3.x_, _local3.y_);
                return;
            case ShowEffect.POISON_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new PoisonEffect(_local3, _arg1.color_);
                _local2.addObj(_local4, _local3.x_, _local3.y_);
                return;
            case ShowEffect.LINE_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new LineEffect(_local3, _arg1.pos1_, _arg1.color_);
                _local2.addObj(_local4, _arg1.pos1_.x_, _arg1.pos1_.y_);
                return;
            case ShowEffect.BURST_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new BurstEffect(_local3, _arg1.pos1_, _arg1.pos2_, _arg1.color_);
                _local2.addObj(_local4, _arg1.pos1_.x_, _arg1.pos1_.y_);
                return;
            case ShowEffect.FLOW_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new FlowEffect(_arg1.pos1_, _local3, _arg1.color_);
                _local2.addObj(_local4, _arg1.pos1_.x_, _arg1.pos1_.y_);
                return;
            case ShowEffect.RING_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new RingEffect(_local3, _arg1.pos1_.x_, _arg1.color_);
                _local2.addObj(_local4, _local3.x_, _local3.y_);
                return;
            case ShowEffect.LIGHTNING_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new LightningEffect(_local3, _arg1.pos1_, _arg1.color_, _arg1.pos2_.x_);
                _local2.addObj(_local4, _local3.x_, _local3.y_);
                return;
            case ShowEffect.COLLAPSE_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new CollapseEffect(_local3, _arg1.pos1_, _arg1.pos2_, _arg1.color_);
                _local2.addObj(_local4, _arg1.pos1_.x_, _arg1.pos1_.y_);
                return;
            case ShowEffect.CONEBLAST_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new ConeBlastEffect(_local3, _arg1.pos1_, _arg1.pos2_.x_, _arg1.color_);
                _local2.addObj(_local4, _local3.x_, _local3.y_);
                return;
            case ShowEffect.JITTER_EFFECT_TYPE:
                gs_.camera_.startJitter();
                return;
            case ShowEffect.FLASH_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local3.flash_ = new FlashDescription(getTimer(), _arg1.color_, _arg1.pos1_.x_, _arg1.pos1_.y_);
                return;
            case ShowEffect.THROW_PROJECTILE_EFFECT_TYPE:
                _local5 = _arg1.pos1_.toPoint();
                if (((!((_local3 == null))) && (!(this.canShowEffect(_local3))))) break;
                _local4 = new ThrowProjectileEffect(_arg1.color_, _arg1.pos2_.toPoint(), _arg1.pos1_.toPoint());
                _local2.addObj(_local4, _local5.x, _local5.y);
                return;
            case ShowEffect.SHOCKER_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                if (((_local3) && (_local3.shockEffect))) {
                    _local3.shockEffect.destroy();
                }
                _local4 = new ShockerEffect(_local3);
                _local3.shockEffect = ShockerEffect(_local4);
                gs_.map.addObj(_local4, _local3.x_, _local3.y_);
                return;
            case ShowEffect.SHOCKEE_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local4 = new ShockeeEffect(_local3);
                gs_.map.addObj(_local4, _local3.x_, _local3.y_);
                return;
            case ShowEffect.RISING_FURY_EFFECT_TYPE:
                _local3 = _local2.goDict_[_arg1.targetObjectId_];
                if ((((_local3 == null)) || (!(this.canShowEffect(_local3))))) break;
                _local6 = (_arg1.pos1_.x_ * 1000);
                _local4 = new RisingFuryEffect(_local3, _local6);
                gs_.map.addObj(_local4, _local3.x_, _local3.y_);
                return;
        }
    }

    private function onGoto(_arg1:Goto):void {
        this.gotoAck(gs_.time);
        var _local2:GameObject = gs_.map.goDict_[_arg1.objectId_];
        if (_local2 == null) {
            return;
        }
        _local2.onGoto(_arg1.pos_.x_, _arg1.pos_.y_, gs_.time);
    }

    private function updateGameObject(_arg1:GameObject, _arg2:Vector.<StatData>, _arg3:Boolean):void {
        var _local7:StatData;
        var _local8:int;
        var _local9:int;
        var _local10:int;
        var _local4:Player = (_arg1 as Player);
        var _local5:Merchant = (_arg1 as Merchant);
        var _local6:Pet = (_arg1 as Pet);
        if (_local6) {
            this.petUpdater.updatePet(_local6, _arg2);
            if (gs_.map.isPetYard) {
                this.petUpdater.updatePetVOs(_local6, _arg2);
            }
            return;
        }
        for each (_local7 in _arg2) {
            _local8 = _local7.statValue_;
            switch (_local7.statType_) {
                case StatData.MAX_HP_STAT:
                    _arg1.maxHP_ = _local8;
                    break;
                case StatData.HP_STAT:
                    _arg1.hp_ = _local8;
                    break;
                case StatData.SIZE_STAT:
                    if (_arg1 is Player)
                        _arg1.setNewSize(_local8);
                    else
                        _arg1.setSize(_local8);
                    break;
                case StatData.MAX_MP_STAT:
                    _local4.maxMP_ = _local8;
                    break;
                case StatData.MP_STAT:
                    _local4.mp_ = _local8;
                    break;
                case StatData.NEXT_LEVEL_EXP_STAT:
                    _local4.nextLevelExp_ = Number(_local7.strStatValue_);
                    break;
                case StatData.NEXT_ATTACK_EXP_STAT:
                    _local4.nextAttackExp_ = Number(_local7.strStatValue_);
                    break;
                case StatData.NEXT_DEFENSE_EXP_STAT:
                    _local4.nextDefenseExp_ = Number(_local7.strStatValue_);
                    break;
                case StatData.EXP_STAT:
                    _local4.exp_ = Number(_local7.strStatValue_);
                    break;
                case StatData.ATTACK_EXP_STAT:
                    _local4.attackExp_ = Number(_local7.strStatValue_);
                    break;
                case StatData.DEFENSE_EXP_STAT:
                    _local4.defenseExp_ = Number(_local7.strStatValue_);
                    break;
                case StatData.LEVEL_STAT:
                    _arg1.level_ = _local8;
                    break;
                case StatData.ATTACK_LEVEL_STAT:
                    _arg1.attackLevel_ = _local8;
                    break;
                case StatData.DEFENSE_LEVEL_STAT:
                    _arg1.defenseLevel_ = _local8;
                    break;
                case StatData.ATTACK_STAT:
                    _local4.attack_ = _local8.toString();
                    break;
                case StatData.DEFENSE_STAT:
                    _arg1.defense_ = _local8.toString();
                    break;
                case StatData.SPEED_STAT:
                    _local4.speed_ = _local8.toString();
                    break;
                case StatData.DEXTERITY_STAT:
                    _local4.dexterity_ = _local8.toString();
                    break;
                case StatData.VITALITY_STAT:
                    _local4.vitality_ = _local8.toString();
                    break;
                case StatData.WISDOM_STAT:
                    _local4.wisdom_ = _local8.toString();
                    break;
                case StatData.CONDITION_STAT:
                    _arg1.condition_[ConditionEffect.CE_FIRST_BATCH] = _local8;
                    break;
                case StatData.INVENTORY_0_STAT:
                case StatData.INVENTORY_1_STAT:
                case StatData.INVENTORY_2_STAT:
                case StatData.INVENTORY_3_STAT:
                case StatData.INVENTORY_4_STAT:
                case StatData.INVENTORY_5_STAT:
                case StatData.INVENTORY_6_STAT:
                case StatData.INVENTORY_7_STAT:
                case StatData.INVENTORY_8_STAT:
                case StatData.INVENTORY_9_STAT:
                case StatData.INVENTORY_10_STAT:
                case StatData.INVENTORY_11_STAT:
                    _local9 = (_local7.statType_ - StatData.INVENTORY_0_STAT);
                    if (_local8 != -1) {
                        _arg1.lockedSlot[_local9] = 0;
                    }
                    _arg1.equipment_[_local9] = _local8;
                    break;
                case StatData.NUM_STARS_STAT:
                    _local4.numStars_ = _local8;
                    break;
                case StatData.NAME_STAT:
                    if (_arg1.name_ != _local7.strStatValue_) {
                        _arg1.name_ = _local7.strStatValue_;
                        _arg1.nameBitmapData_ = null;
                    }
                    break;
                case StatData.TEX1_STAT:
                    (((_local8 >= 0)) && (_arg1.setTex1(_local8)));
                    break;
                case StatData.TEX2_STAT:
                    (((_local8 >= 0)) && (_arg1.setTex2(_local8)));
                    break;
                case StatData.MERCHANDISE_TYPE_STAT:
                    _local5.setMerchandiseType(_local8);
                    break;
                case StatData.CREDITS_STAT:
                    _local4.setCredits(_local8);
                    break;
                case StatData.MERCHANDISE_PRICE_STAT:
                    (_arg1 as SellableObject).setPrice(_local8);
                    break;
                case StatData.ACTIVE_STAT:
                    (_arg1 as Portal).active_ = !((_local8 == 0));
                    break;
                case StatData.ACCOUNT_ID_STAT:
                    _local4.accountId_ = _local7.strStatValue_;
                    break;
                case StatData.FAME_STAT:
                    _local4.fame_ = Number(_local7.strStatValue_);
                    break;
                case StatData.FORTUNE_TOKEN_STAT:
                    _local4.setTokens(_local8);
                    break;
                case StatData.MERCHANDISE_CURRENCY_STAT:
                    (_arg1 as SellableObject).setCurrency(_local8);
                    break;
                case StatData.CONNECT_STAT:
                    _arg1.connectType_ = _local8;
                    break;
                case StatData.MERCHANDISE_COUNT_STAT:
                    _local5.count_ = _local8;
                    _local5.untilNextMessage_ = 0;
                    break;
                case StatData.MERCHANDISE_MINS_LEFT_STAT:
                    _local5.minsLeft_ = _local8;
                    _local5.untilNextMessage_ = 0;
                    break;
                case StatData.MERCHANDISE_DISCOUNT_STAT:
                    _local5.discount_ = _local8;
                    _local5.untilNextMessage_ = 0;
                    break;
                case StatData.MERCHANDISE_RANK_REQ_STAT:
                    (_arg1 as SellableObject).setRankReq(_local8);
                    break;
                case StatData.MAX_HP_BOOST_STAT:
                    _local4.maxHPBoost_ = _local8.toString();
                    break;
                case StatData.MAX_MP_BOOST_STAT:
                    _local4.maxMPBoost_ = _local8.toString();
                    break;
                case StatData.ATTACK_BOOST_STAT:
                    _local4.attackBoost_ = _local8.toString();
                    break;
                case StatData.DEFENSE_BOOST_STAT:
                    _local4.defenseBoost_ = _local8.toString();
                    break;
                case StatData.SPEED_BOOST_STAT:
                    _local4.speedBoost_ = _local8.toString();
                    break;
                case StatData.VITALITY_BOOST_STAT:
                    _local4.vitalityBoost_ = _local8.toString();
                    break;
                case StatData.WISDOM_BOOST_STAT:
                    _local4.wisdomBoost_ = _local8.toString();
                    break;
                case StatData.DEXTERITY_BOOST_STAT:
                    _local4.dexterityBoost_ = _local8.toString();
                    break;
                case StatData.OWNER_ACCOUNT_ID_STAT:
                    (_arg1 as Container).setOwnerId(_local7.strStatValue_);
                    break;
                case StatData.RANK_REQUIRED_STAT:
                    (_arg1 as NameChanger).setRankRequired(_local8);
                    break;
                case StatData.NAME_CHOSEN_STAT:
                    _local4.nameChosen_ = !((_local8 == 0));
                    _arg1.nameBitmapData_ = null;
                    break;
                case StatData.CURR_FAME_STAT:
                    _local4.currFame_ = Number(_local7.strStatValue_);
                    break;
                case StatData.NEXT_CLASS_QUEST_FAME_STAT:
                    _local4.nextClassQuestFame_ = Number(_local7.strStatValue_);
                    break;
                case StatData.GLOW_COLOR_STAT:
                    _local4.setGlow(_local8);
                    break;
                case StatData.SINK_LEVEL_STAT:
                    if (!_arg3) {
                        _local4.sinkLevel_ = _local8;
                    }
                    break;
                case StatData.ALT_TEXTURE_STAT:
                    _arg1.setAltTexture(_local8);
                    break;
                case StatData.GUILD_NAME_STAT:
                    _local4.setGuildName(_local7.strStatValue_);
                    break;
                case StatData.GUILD_RANK_STAT:
                    _local4.guildRank_ = _local8;
                    break;
                case StatData.BREATH_STAT:
                    _local4.breath_ = _local8;
                    break;
                case StatData.XP_BOOSTED_STAT:
                    _local4.xpBoost_ = _local8;
                    break;
                case StatData.XP_TIMER_STAT:
                    _local4.xpTimer = (_local8 * TO_MILLISECONDS);
                    break;
                case StatData.LD_TIMER_STAT:
                    _local4.dropBoost = (_local8 * TO_MILLISECONDS);
                    break;
                case StatData.LT_TIMER_STAT:
                    _local4.tierBoost = (_local8 * TO_MILLISECONDS);
                    break;
                case StatData.HEALTH_POTION_STACK_STAT:
                    _local4.healthPotionCount_ = _local8;
                    break;
                case StatData.MAGIC_POTION_STACK_STAT:
                    _local4.magicPotionCount_ = _local8;
                    break;
                case StatData.TEXTURE_STAT:
                    ((((!((_local4.skinId == _local8))) && ((_local8 >= 0)))) && (this.setPlayerSkinTemplate(_local4, _local8)));
                    break;
                case StatData.HASBACKPACK_STAT:
                    (_arg1 as Player).hasBackpack_ = Boolean(_local8);
                    if (_arg3) {
                        this.updateBackpackTab.dispatch(Boolean(_local8));
                    }
                    break;
                case StatData.BACKPACK_0_STAT:
                case StatData.BACKPACK_1_STAT:
                case StatData.BACKPACK_2_STAT:
                case StatData.BACKPACK_3_STAT:
                case StatData.BACKPACK_4_STAT:
                case StatData.BACKPACK_5_STAT:
                case StatData.BACKPACK_6_STAT:
                case StatData.BACKPACK_7_STAT:
                    _local10 = (((_local7.statType_ - StatData.BACKPACK_0_STAT) + GeneralConstants.NUM_EQUIPMENT_SLOTS) + GeneralConstants.NUM_INVENTORY_SLOTS);
                    (_arg1 as Player).equipment_[_local10] = _local8;
                    break;
                case StatData.NEW_CON_STAT:
                    _arg1.condition_[ConditionEffect.CE_SECOND_BATCH] = _local8;
                    break;
                case StatData.ACCOUNT_TYPE:
                    _local4.accountType_ = _local8;
                    break;
                case StatData.ADMIN:
                    _local4.admin_ = _local8 == 1;
                    break;
                case StatData.PET_OBJECT_ID:
                    _local4.petObjectId = _local8;
                    break;
                case StatData.PET_HP_HEALING_AVERAGE_MIN:
                    _local4.petHpHealingAverageMin = _local8;
                    break;
                case StatData.PET_HP_HEALING_AVERAGE_MAX:
                    _local4.petHpHealingAverageMax = _local8;
                    break;
                case StatData.PET_HP_HEALING_AVERAGE_BONUS:
                    _local4.petHpHealingAverageBonus = _local8;
                    break;
                case StatData.PET_MP_HEALING_AVERAGE_MIN:
                    _local4.petMpHealingAverageMin = _local8;
                    break;
                case StatData.PET_MP_HEALING_AVERAGE_MAX:
                    _local4.petMpHealingAverageMax = _local8;
                    break;
                case StatData.PET_MP_HEALING_AVERAGE_BONUS:
                    _local4.petMpHealingAverageBonus = _local8;
                    break;
                case StatData.PET_ATTACK_COOLDOWN:
                    _local4.petAttackCooldown = _local8;
                    break;
                case StatData.PET_ATTACK_CHANCE:
                    _local4.petAttackChance = _local8;
                    break;
                case StatData.PET_ATTACK_DAMAGE_MIN:
                    _local4.petAttackDamageMin = _local8;
                    break;
                case StatData.PET_ATTACK_DAMAGE_MAX:
                    _local4.petAttackDamageMax = _local8;
                    break;
            }
        }
    }

    private function setPlayerSkinTemplate(_arg1:Player, _arg2:int):void {
        var _local3:Reskin = (this.messages.require(RESKIN) as Reskin);
        _local3.skinID = _arg2;
        _local3.player = _arg1;
        _local3.consume();
    }

    private function processObjectStatus(_arg1:ObjectStatusData, _arg2:int, _arg3:int):void {
        var _local8:int;
        var _local9:Number;
        var _local10:int;
        var _local11:CharacterClass;
        var _local12:XML;
        var _local13:String;
        var _local14:String;
        var _local15:int;
        var _local16:ObjectProperties;
        var _local17:ProjectileProperties;
        var _local18:Array;
        var _local4:AbstractMap = gs_.map;
        var _local5:GameObject = _local4.goDict_[_arg1.objectId_];
        if (_local5 == null) {
            return;
        }
        var _local6:Boolean = (_arg1.objectId_ == this.playerId_);
        if (((!((_arg2 == 0))) && (!(_local6)))) {
            _local5.onTickPos(_arg1.pos_.x_, _arg1.pos_.y_, _arg2, _arg3);
        }
        var _local7:Player = (_local5 as Player);
        if (_local7 != null) {
            _local8 = _local7.level_;
            _local9 = _local7.exp_;
            _local10 = _local7.skinId;
        }
        this.updateGameObject(_local5, _arg1.stats_, _local6);
        if (_local7) {
            if (_local6) {
                _local11 = this.classesModel.getCharacterClass(_local7.objectType_);
                if (_local11.getMaxLevelAchieved() < _local7.level_) {
                    _local11.setMaxLevelAchieved(_local7.level_);
                }
            }
            if (_local7.skinId != _local10) {
                if (ObjectLibrary.skinSetXMLDataLibrary_[_local7.skinId] != null) {
                    _local12 = (ObjectLibrary.skinSetXMLDataLibrary_[_local7.skinId] as XML);
                    _local13 = _local12.attribute("color");
                    _local14 = _local12.attribute("bulletType");
                    if (((!((_local8 == -1))) && ((_local13.length > 0)))) {
                        _local7.levelUpParticleEffect(uint(_local13));
                    }
                    if (_local14.length > 0) {
                        _local7.projectileIdSetOverrideNew = _local14;
                        _local15 = _local7.equipment_[0];
                        _local16 = ObjectLibrary.propsLibrary_[_local15];
                        _local17 = _local16.projectiles_[0];
                        _local7.projectileIdSetOverrideOld = _local17.objectId_;
                    }
                }
                else {
                    if (ObjectLibrary.skinSetXMLDataLibrary_[_local7.skinId] == null) {
                        _local7.projectileIdSetOverrideNew = "";
                        _local7.projectileIdSetOverrideOld = "";
                    }
                }
            }
            if (((!((_local8 == -1))) && ((_local7.level_ > _local8)))) {
                if (_local6) {
                    _local18 = gs_.model.getNewUnlocks(_local7.objectType_, _local7.level_);
                    _local7.handleLevelUp(!((_local18.length == 0)));
                }
                else {
                    _local7.levelUpEffect(TextKey.PLAYER_LEVELUP);
                }
            }
            else {
                if (((!((_local8 == -1))) && ((_local7.exp_ > _local9)))) {
                    _local7.handleExpUp((_local7.exp_ - _local9));
                }
            }
            this.friendModel.updateFriendVO(_local7.getName(), _local7);
        }
    }

    private function onInvResult(_arg1:InvResult):void {
        if (_arg1.result_ != 0) {
            this.handleInvFailure();
        }
    }

    private function handleInvFailure():void {
        SoundEffectLibrary.play("error");
        gs_.hudView.interactPanel.redraw();
    }

    private function onReconnect(_arg1:Reconnect):void {
        var _local2:Server = new Server().setName(_arg1.name_).setAddress((((_arg1.host_) != "") ? _arg1.host_ : server_.address)).setPort((((_arg1.host_) != "") ? _arg1.port_ : server_.port));
        var _local3:int = _arg1.gameId_;
        var _local4:Boolean = createCharacter_;
        var _local5:int = charId_;
        var _local6:int = _arg1.keyTime_;
        var _local7:ByteArray = _arg1.key_;
        isFromArena_ = _arg1.isFromArena_;
        var _local8:ReconnectEvent = new ReconnectEvent(_local2, _local3, _local4, _local5, _local6, _local7, isFromArena_);
        gs_.dispatchEvent(_local8);
    }

    private function onPing(ping:Ping):void {
        var pong:Pong = (this.messages.require(PONG) as Pong);
        pong.serial_ = ping.serial_;
        pong.time_ = getTimer();

        serverConnection.queueMessage(pong);
    }

    private function parseXML(_arg1:String):void {
        var _local2:XML = XML(_arg1);
        GroundLibrary.parseFromXML(_local2);
        ObjectLibrary.parseFromXML(_local2);
        ObjectLibrary.parseFromXML(_local2);
    }

    private function onMapInfo(_arg1:MapInfo):void {
        var _local2:String;
        var _local3:String;
        for each (_local2 in _arg1.clientXML_) {
            this.parseXML(_local2);
        }
        for each (_local3 in _arg1.extraXML_) {
            this.parseXML(_local3);
        }
        changeMapSignal.dispatch();
        this.closeDialogs.dispatch();
        gs_.applyMapInfo(_arg1);
        this.rand_ = new Random(_arg1.fp_);
        Music.load(_arg1.music);
        if (createCharacter_) {
            this.create();
        }
        else {
            this.load();
        }
    }

    private function onPic(_arg1:Pic):void {
        gs_.addChild(new PicView(_arg1.bitmapData_));
    }

    private function onDeath(_arg1:Death):void {
        this.death = _arg1;
        var _local2:BitmapData = new BitmapDataSpy(gs_.stage.stageWidth, gs_.stage.stageHeight);
        _local2.draw(gs_);
        _arg1.background = _local2;
        if (!gs_.isEditor) {
            this.handleDeath.dispatch(_arg1);
        }
        this.checkDavyKeyRemoval();
    }

    private function onBuyResult(_arg1:BuyResult):void {
        outstandingBuy_ = false;
        this.handleBuyResultType(_arg1);
    }

    private function handleBuyResultType(_arg1:BuyResult):void {
        var _local2:ChatMessage;
        switch (_arg1.result_) {
            case BuyResult.UNKNOWN_ERROR_BRID:
                _local2 = ChatMessage.make(Parameters.SERVER_CHAT_NAME, _arg1.resultString_);
                this.addTextLine.dispatch(_local2);
                return;
            case BuyResult.NOT_ENOUGH_GOLD_BRID:
                this.openDialog.dispatch(new NotEnoughGoldDialog());
                return;
            case BuyResult.NOT_ENOUGH_FAME_BRID:
                this.openDialog.dispatch(new NotEnoughFameDialog());
                return;
            default:
                this.handleDefaultResult(_arg1);
        }
    }

    private function handleDefaultResult(_arg1:BuyResult):void {
        var _local2:LineBuilder = LineBuilder.fromJSON(_arg1.resultString_);
        var _local3:Boolean = (((_arg1.result_ == BuyResult.SUCCESS_BRID)) || ((_arg1.result_ == BuyResult.PET_FEED_SUCCESS_BRID)));
        var _local4:ChatMessage = ChatMessage.make(_local3 ? Parameters.SERVER_CHAT_NAME : Parameters.ERROR_CHAT_NAME, _local2.key);
        _local4.tokens = _local2.tokens;
        this.addTextLine.dispatch(_local4);
    }

    private function onAccountList(_arg1:AccountList):void {
        if (_arg1.accountListId_ == 0) {
            if (_arg1.lockAction_ != -1) {
                if (_arg1.lockAction_ == 1) {
                    gs_.map.party_.setStars(_arg1);
                }
                else {
                    gs_.map.party_.removeStars(_arg1);
                }
            }
            else {
                gs_.map.party_.setStars(_arg1);
            }
        }
        else {
            if (_arg1.accountListId_ == 1) {
                gs_.map.party_.setIgnores(_arg1);
            }
        }
    }

    private function onQuestObjId(_arg1:QuestObjId):void {
        gs_.map.quest_.setObject(_arg1.objectId_);
    }

    private function onAoe(_arg1:Aoe):void {
        var _local4:int;
        var _local5:Vector.<uint>;
        if (this.player == null) {
            this.aoeAck(gs_.time, 0, 0);
            return;
        }
        var _local2:AOEEffect = new AOEEffect(_arg1.pos_.toPoint(), _arg1.radius_, 0xFF0000);
        gs_.map.addObj(_local2, _arg1.pos_.x_, _arg1.pos_.y_);
        if (((this.player.isInvincible()) || (this.player.isPaused()))) {
            this.aoeAck(gs_.time, this.player.x_, this.player.y_);
            return;
        }
        var _local3:Boolean = (this.player.distTo(_arg1.pos_) < _arg1.radius_);
        if (_local3) {
            _local4 = GameObject.damageWithDefense(_arg1.damage_, this.player.defenseLevel_ + Parameters.parse(this.player.defenseBoost_), false, this.player.condition_);
            _local5 = null;
            if (_arg1.effect_ != 0) {
                _local5 = new Vector.<uint>();
                _local5.push(_arg1.effect_);
            }
            this.player.damage(_arg1.origType_, _local4, _local5, false, null);
        }
        this.aoeAck(gs_.time, this.player.x_, this.player.y_);
    }

    private function onNameResult(_arg1:NameResult):void {
        gs_.dispatchEvent(new NameResultEvent(_arg1));
    }

    private function onGuildResult(_arg1:GuildResult):void {
        var _local2:LineBuilder;
        if (_arg1.lineBuilderJSON == "") {
            gs_.dispatchEvent(new GuildResultEvent(_arg1.success_, "", {}));
        }
        else {
            _local2 = LineBuilder.fromJSON(_arg1.lineBuilderJSON);
            this.addTextLine.dispatch(ChatMessage.make(Parameters.ERROR_CHAT_NAME, _local2.key, -1, -1, "", false, _local2.tokens));
            gs_.dispatchEvent(new GuildResultEvent(_arg1.success_, _local2.key, _local2.tokens));
        }
    }

    private function onClientStat(_arg1:ClientStat):void {
        var _local2:Account = StaticInjectorContext.getInjector().getInstance(Account);
        _local2.reportIntStat(_arg1.name_, _arg1.value_);
    }

    private function onFile(_arg1:File):void {
        new FileReference().save(_arg1.file_, _arg1.filename_);
    }

    private function onInvitedToGuild(_arg1:InvitedToGuild):void {
        if (Parameters.data_.showGuildInvitePopup) {
            gs_.hudView.interactPanel.setOverride(new GuildInvitePanel(gs_, _arg1.name_, _arg1.guildName_));
        }
        this.addTextLine.dispatch(ChatMessage.make("", (((((("You have been invited by " + _arg1.name_) + " to join the guild ") + _arg1.guildName_) + '.\n  If you wish to join type "/join ') + _arg1.guildName_) + '"')));
    }

    private function onPlaySound(_arg1:PlaySound):void {
        var _local2:GameObject = gs_.map.goDict_[_arg1.ownerId_];
        ((_local2) && (_local2.playSound(_arg1.soundId_)));
    }

    private function onImminentArenaWave(_arg1:ImminentArenaWave):void {
        this.imminentWave.dispatch(_arg1.currentRuntime);
    }

    private function onArenaDeath(_arg1:ArenaDeath):void {
        this.currentArenaRun.costOfContinue = _arg1.cost;
        this.openDialog.dispatch(new ContinueOrQuitDialog(_arg1.cost, false));
        this.arenaDeath.dispatch();
    }

    private function onVerifyEmail(_arg1:VerifyEmail):void {
        TitleView.queueEmailConfirmation = true;
        if (gs_ != null) {
            gs_.closed.dispatch();
        }
        var _local2:HideMapLoadingSignal = StaticInjectorContext.getInjector().getInstance(HideMapLoadingSignal);
        if (_local2 != null) {
            _local2.dispatch();
        }
    }

    private function onPasswordPrompt(_arg1:PasswordPrompt):void {
        if (_arg1.cleanPasswordStatus == 3) {
            TitleView.queuePasswordPromptFull = true;
        }
        else {
            if (_arg1.cleanPasswordStatus == 2) {
                TitleView.queuePasswordPrompt = true;
            }
            else {
                if (_arg1.cleanPasswordStatus == 4) {
                    TitleView.queueRegistrationPrompt = true;
                }
            }
        }
        if (gs_ != null) {
            gs_.closed.dispatch();
        }
        var _local2:HideMapLoadingSignal = StaticInjectorContext.getInjector().getInstance(HideMapLoadingSignal);
        if (_local2 != null) {
            _local2.dispatch();
        }
    }

    override public function questFetch():void {
        serverConnection.queueMessage(this.messages.require(QUEST_FETCH_ASK));
    }

    private function onQuestFetchResponse(_arg1:QuestFetchResponse):void {
        this.questFetchComplete.dispatch(_arg1);
    }

    private function onQuestRedeemResponse(_arg1:QuestRedeemResponse):void {
        this.questRedeemComplete.dispatch(_arg1);
    }

    override public function questRedeem(objectId:int, slotId:int, objectType:int):void {
        var questRedeem:QuestRedeem = (this.messages.require(QUEST_REDEEM) as QuestRedeem);
        questRedeem.slotObject.objectId_ = objectId;
        questRedeem.slotObject.slotId_ = slotId;
        questRedeem.slotObject.objectType_ = objectType;

        serverConnection.queueMessage(questRedeem);
    }

    override public function keyInfoRequest(itemType:int):void {
        var keyInfoRequest:KeyInfoRequest = (this.messages.require(KEY_INFO_REQUEST) as KeyInfoRequest);
        keyInfoRequest.itemType_ = itemType;

        serverConnection.queueMessage(keyInfoRequest);
    }

    private function onKeyInfoResponse(_arg1:KeyInfoResponse):void {
        this.keyInfoResponse.dispatch(_arg1);
    }

    private function onLoginRewardResponse(_arg1:ClaimDailyRewardResponse):void {
        this.claimDailyRewardResponse.dispatch(_arg1);
    }

    private function onClosed():void {
        if (this.playerId_ != -1) {
            gs_.closed.dispatch();
        }
        else {
            if (this.retryConnection_) {
                if (this.delayBeforeReconnect <= 100) {
                    this.retry(this.delayBeforeReconnect++);
                    this.addTextLine.dispatch(ChatMessage.make(Parameters.ERROR_CHAT_NAME, "[" + this.delayBeforeReconnect + "/100 Attempts] Connection failed!  Retrying..."));
                }
                else {
                    gs_.closed.dispatch();
                }
            }
        }
    }

    private function retry(delay:int):void {
        this.retryTimer_ = new Timer(1000 * delay, 1);
        this.retryTimer_.addEventListener(TimerEvent.TIMER_COMPLETE, this.SocketReconnect);
        this.retryTimer_.start();
    }

    private function SocketReconnect(_arg1:TimerEvent):void {
        serverConnection.connect(server_.address, server_.port);
    }

    private function onError(_arg1:String):void {
        // remove loading screen
        var hideLoadingScreen:Signal = this.injector.getInstance(HideMapLoadingSignal);
        hideLoadingScreen && hideLoadingScreen.dispatch();

        this.addTextLine.dispatch(ChatMessage.make(Parameters.ERROR_CHAT_NAME, _arg1));
    }

    private function onFailure(_arg1:Failure):void {
        // remove loading screen
        var hideLoadingScreen:Signal = this.injector.getInstance(HideMapLoadingSignal);
        hideLoadingScreen && hideLoadingScreen.dispatch();

        switch (_arg1.errorId_) {
            case Failure.BAD_KEY:
                this.handleBadKeyFailure(_arg1);
                return;
            case Failure.INVALID_TELEPORT_TARGET:
                this.handleInvalidTeleportTarget(_arg1);
                return;
            case Failure.EMAIL_VERIFICATION_NEEDED:
                this.handleEmailVerificationNeeded(_arg1);
                return;
            case Failure.JSON_DIALOG:
                this.handleJsonDialog(_arg1);
                return;
            default:
                this.handleDefaultFailure(_arg1);
        }
    }

    private function handleJsonDialog(_arg1:Failure):void {
        var errorMsg:Object = JSON.parse(_arg1.errorDescription_);
        var dlg:Dialog;

        // correct version, display custom json dialog
        dlg = new Dialog(errorMsg.title, errorMsg.description, "Ok", null, null, errorMsg.titleColor);
        dlg.addEventListener(Dialog.LEFT_BUTTON, this.onDoClientUpdate);
        this.gs_.addChild(dlg);
        this.retryConnection_ = false;
    }

    private function handleEmailVerificationNeeded(_arg1:Failure):void {
        this.retryConnection_ = false;
        gs_.closed.dispatch();
    }

    private function handleInvalidTeleportTarget(_arg1:Failure):void {
        var _local2:String = LineBuilder.getLocalizedStringFromJSON(_arg1.errorDescription_);
        if (_local2 == "") {
            _local2 = _arg1.errorDescription_;
        }
        this.addTextLine.dispatch(ChatMessage.make(Parameters.ERROR_CHAT_NAME, _local2));
        this.player.nextTeleportAt_ = 0;
    }

    private function handleBadKeyFailure(_arg1:Failure):void {
        var _local2:String = LineBuilder.getLocalizedStringFromJSON(_arg1.errorDescription_);
        if (_local2 == "") {
            _local2 = _arg1.errorDescription_;
        }
        this.addTextLine.dispatch(ChatMessage.make(Parameters.ERROR_CHAT_NAME, _local2));
        this.retryConnection_ = false;
        gs_.closed.dispatch();
    }

    private function handleDefaultFailure(_arg1:Failure):void {
        var _local2:String = LineBuilder.getLocalizedStringFromJSON(_arg1.errorDescription_);
        if (_local2 == "") {
            _local2 = _arg1.errorDescription_;
        }
        this.addTextLine.dispatch(ChatMessage.make(Parameters.ERROR_CHAT_NAME, _local2));
    }

    private function onDoClientUpdate(_arg1:Event):void {
        var _local2:Dialog = (_arg1.currentTarget as Dialog);
        _local2.parent.removeChild(_local2);
        gs_.closed.dispatch();
    }

    override public function isConnected():Boolean {
        return (serverConnection.isConnected());
    }

    private function setFocus(pkt:SetFocus):void {
        var goDict:Dictionary = this.gs_.map.goDict_;
        if (goDict) {
            var go:GameObject = goDict[pkt.objectId_];
            gs_.setFocus(go);
            gs_.hudView.setMiniMapFocus(go);
        }
    }


}
}
