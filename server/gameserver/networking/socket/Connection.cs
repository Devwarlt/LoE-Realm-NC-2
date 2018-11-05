using LoESoft.Core.models;
using LoESoft.GameServer.networking.error;
using LoESoft.GameServer.networking.outgoing;
using System;
using FAILURE = LoESoft.GameServer.networking.outgoing.FAILURE;

namespace LoESoft.GameServer.networking
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public partial class Client
    {
        public enum DisconnectReason : byte
        {
            // Unregistered DisconnectReason '0',
            FAILED_TO_LOAD_CHARACTER = 1,

            OUTDATED_CLIENT = 2,
            DISABLE_GUEST_ACCOUNT = 3,
            BAD_LOGIN = 4,
            SERVER_FULL = 5,
            ACCOUNT_BANNED = 6,
            INVALID_DISCONNECT_KEY = 7,
            LOST_CONNECTION = 8,
            ACCOUNT_IN_USE = 9,
            INVALID_WORLD = 10,
            INVALID_PORTAL_KEY = 11,
            PORTAL_KEY_EXPIRED = 12,
            CHARACTER_IS_DEAD = 13,
            HP_POTION_CHEAT_ENGINE = 14,
            MP_POTION_CHEAT_ENGINE = 15,
            STOPING_SERVER = 16,
            SOCKET_IS_NOT_CONNECTED = 17,
            INVALID_MESSAGE_LENGTH = 18,
            RECEIVING_DATA = 19,
            ERROR_WHEN_HANDLING_MESSAGE = 20,
            SOCKET_ERROR_DETECTED = 21,
            PROCESS_POLICY_FILE = 22,
            RESTART = 23,
            PLAYER_KICK = 24,
            PLAYER_BANNED = 25,
            CHARACTER_IS_DEAD_ERROR = 26,
            CHEAT_ENGINE_DETECTED = 27,
            RECONNECT_TO_CASTLE = 28,
            REALM_MANAGER_DISCONNECT = 29,
            STOPPING_REALM_MANAGER = 30,
            DUPER_DISCONNECT = 31,
            ACCESS_DENIED = 32,
            VIP_ACCOUNT_OVER = 33,
            DEXTERITY_HACK_MOD = 34,
            RECONNECT = 35,
            CONNECTION_RESET = 36,
            SOCKET_ERROR = 37,
            CONNECTION_LOST = 38,
            OVERFLOW_EXCEPTION = 39,
            NETWORK_TICKER_DISCONNECT = 40,
            OLD_CLIENT_DISCONNECT = 41,
            BYTES_NOT_READY = 42,
            SERVER_MODE_LOCAL_ONLY = 43,
            SERVER_MODE_CLOSED_TEST_ONLY = 44,
            SERVER_MODE_PRODUCTION_ONLY = 45,

            // Unregistered DisconnectReason '46',
            // Unregistered DisconnectReason '47',
            // Unregistered DisconnectReason '48',
            // Unregistered DisconnectReason '49',
            // Unregistered DisconnectReason '50',
            // Unregistered DisconnectReason '51',
            // Unregistered DisconnectReason '52',
            // Unregistered DisconnectReason '53',
            // Unregistered DisconnectReason '54',
            // Unregistered DisconnectReason '55',
            // Unregistered DisconnectReason '56',
            // Unregistered DisconnectReason '57',
            // Unregistered DisconnectReason '58',
            // Unregistered DisconnectReason '59',
            // Unregistered DisconnectReason '60',
            // Unregistered DisconnectReason '61',
            // Unregistered DisconnectReason '62',
            // Unregistered DisconnectReason '63',
            // Unregistered DisconnectReason '64',
            // Unregistered DisconnectReason '65',
            // Unregistered DisconnectReason '66',
            // Unregistered DisconnectReason '67',
            // Unregistered DisconnectReason '68',
            // Unregistered DisconnectReason '69',
            // Unregistered DisconnectReason '70',
            // Unregistered DisconnectReason '71',
            // Unregistered DisconnectReason '72',
            // Unregistered DisconnectReason '73',
            // Unregistered DisconnectReason '74',
            // Unregistered DisconnectReason '75',
            // Unregistered DisconnectReason '76',
            // Unregistered DisconnectReason '77',
            // Unregistered DisconnectReason '78',
            // Unregistered DisconnectReason '79',
            // Unregistered DisconnectReason '80',
            // Unregistered DisconnectReason '81',
            // Unregistered DisconnectReason '82',
            // Unregistered DisconnectReason '83',
            // Unregistered DisconnectReason '84',
            // Unregistered DisconnectReason '85',
            // Unregistered DisconnectReason '86',
            // Unregistered DisconnectReason '87',
            // Unregistered DisconnectReason '88',
            // Unregistered DisconnectReason '89',
            // Unregistered DisconnectReason '90',
            // Unregistered DisconnectReason '91',
            // Unregistered DisconnectReason '92',
            // Unregistered DisconnectReason '93',
            // Unregistered DisconnectReason '94',
            // Unregistered DisconnectReason '95',
            // Unregistered DisconnectReason '96',
            // Unregistered DisconnectReason '97',
            // Unregistered DisconnectReason '98',
            // Unregistered DisconnectReason '99',
            // Unregistered DisconnectReason '100',
            // Unregistered DisconnectReason '101',
            // Unregistered DisconnectReason '102',
            // Unregistered DisconnectReason '103',
            // Unregistered DisconnectReason '104',
            // Unregistered DisconnectReason '105',
            // Unregistered DisconnectReason '106',
            // Unregistered DisconnectReason '107',
            // Unregistered DisconnectReason '108',
            // Unregistered DisconnectReason '109',
            // Unregistered DisconnectReason '110',
            // Unregistered DisconnectReason '111',
            // Unregistered DisconnectReason '112',
            // Unregistered DisconnectReason '113',
            // Unregistered DisconnectReason '114',
            // Unregistered DisconnectReason '115',
            // Unregistered DisconnectReason '116',
            // Unregistered DisconnectReason '117',
            // Unregistered DisconnectReason '118',
            // Unregistered DisconnectReason '119',
            // Unregistered DisconnectReason '120',
            // Unregistered DisconnectReason '121',
            // Unregistered DisconnectReason '122',
            // Unregistered DisconnectReason '123',
            // Unregistered DisconnectReason '124',
            // Unregistered DisconnectReason '125',
            // Unregistered DisconnectReason '126',
            // Unregistered DisconnectReason '127',
            // Unregistered DisconnectReason '128',
            // Unregistered DisconnectReason '129',
            // Unregistered DisconnectReason '130',
            // Unregistered DisconnectReason '131',
            // Unregistered DisconnectReason '132',
            // Unregistered DisconnectReason '133',
            // Unregistered DisconnectReason '134',
            // Unregistered DisconnectReason '135',
            // Unregistered DisconnectReason '136',
            // Unregistered DisconnectReason '137',
            // Unregistered DisconnectReason '138',
            // Unregistered DisconnectReason '139',
            // Unregistered DisconnectReason '140',
            // Unregistered DisconnectReason '141',
            // Unregistered DisconnectReason '142',
            // Unregistered DisconnectReason '143',
            // Unregistered DisconnectReason '144',
            // Unregistered DisconnectReason '145',
            // Unregistered DisconnectReason '146',
            // Unregistered DisconnectReason '147',
            // Unregistered DisconnectReason '148',
            // Unregistered DisconnectReason '149',
            // Unregistered DisconnectReason '150',
            // Unregistered DisconnectReason '151',
            // Unregistered DisconnectReason '152',
            // Unregistered DisconnectReason '153',
            // Unregistered DisconnectReason '154',
            // Unregistered DisconnectReason '155',
            // Unregistered DisconnectReason '156',
            // Unregistered DisconnectReason '157',
            // Unregistered DisconnectReason '158',
            // Unregistered DisconnectReason '159',
            // Unregistered DisconnectReason '160',
            // Unregistered DisconnectReason '161',
            // Unregistered DisconnectReason '162',
            // Unregistered DisconnectReason '163',
            // Unregistered DisconnectReason '164',
            // Unregistered DisconnectReason '165',
            // Unregistered DisconnectReason '166',
            // Unregistered DisconnectReason '167',
            // Unregistered DisconnectReason '168',
            // Unregistered DisconnectReason '169',
            // Unregistered DisconnectReason '170',
            // Unregistered DisconnectReason '171',
            // Unregistered DisconnectReason '172',
            // Unregistered DisconnectReason '173',
            // Unregistered DisconnectReason '174',
            // Unregistered DisconnectReason '175',
            // Unregistered DisconnectReason '176',
            // Unregistered DisconnectReason '177',
            // Unregistered DisconnectReason '178',
            // Unregistered DisconnectReason '179',
            // Unregistered DisconnectReason '180',
            // Unregistered DisconnectReason '181',
            // Unregistered DisconnectReason '182',
            // Unregistered DisconnectReason '183',
            // Unregistered DisconnectReason '184',
            // Unregistered DisconnectReason '185',
            // Unregistered DisconnectReason '186',
            // Unregistered DisconnectReason '187',
            // Unregistered DisconnectReason '188',
            // Unregistered DisconnectReason '189',
            // Unregistered DisconnectReason '190',
            // Unregistered DisconnectReason '191',
            // Unregistered DisconnectReason '192',
            // Unregistered DisconnectReason '193',
            // Unregistered DisconnectReason '194',
            // Unregistered DisconnectReason '195',
            // Unregistered DisconnectReason '196',
            // Unregistered DisconnectReason '197',
            // Unregistered DisconnectReason '198',
            // Unregistered DisconnectReason '199',
            // Unregistered DisconnectReason '200',
            // Unregistered DisconnectReason '201',
            // Unregistered DisconnectReason '202',
            // Unregistered DisconnectReason '203',
            // Unregistered DisconnectReason '204',
            // Unregistered DisconnectReason '205',
            // Unregistered DisconnectReason '206',
            // Unregistered DisconnectReason '207',
            // Unregistered DisconnectReason '208',
            // Unregistered DisconnectReason '209',
            // Unregistered DisconnectReason '210',
            // Unregistered DisconnectReason '211',
            // Unregistered DisconnectReason '212',
            // Unregistered DisconnectReason '213',
            // Unregistered DisconnectReason '214',
            // Unregistered DisconnectReason '215',
            // Unregistered DisconnectReason '216',
            // Unregistered DisconnectReason '217',
            // Unregistered DisconnectReason '218',
            // Unregistered DisconnectReason '219',
            // Unregistered DisconnectReason '220',
            // Unregistered DisconnectReason '221',
            // Unregistered DisconnectReason '222',
            // Unregistered DisconnectReason '223',
            // Unregistered DisconnectReason '224',
            // Unregistered DisconnectReason '225',
            // Unregistered DisconnectReason '226',
            // Unregistered DisconnectReason '227',
            // Unregistered DisconnectReason '228',
            // Unregistered DisconnectReason '229',
            // Unregistered DisconnectReason '230',
            // Unregistered DisconnectReason '231',
            // Unregistered DisconnectReason '232',
            // Unregistered DisconnectReason '233',
            // Unregistered DisconnectReason '234',
            // Unregistered DisconnectReason '235',
            // Unregistered DisconnectReason '236',
            // Unregistered DisconnectReason '237',
            // Unregistered DisconnectReason '238',
            // Unregistered DisconnectReason '239',
            // Unregistered DisconnectReason '240',
            // Unregistered DisconnectReason '241',
            // Unregistered DisconnectReason '242',
            // Unregistered DisconnectReason '243',
            // Unregistered DisconnectReason '244',
            // Unregistered DisconnectReason '245',
            // Unregistered DisconnectReason '246',
            // Unregistered DisconnectReason '247',
            // Unregistered DisconnectReason '248',
            // Unregistered DisconnectReason '249',
            // Unregistered DisconnectReason '250',
            // Unregistered DisconnectReason '251',
            // Unregistered DisconnectReason '252',
            // Unregistered DisconnectReason '253',
            // Unregistered DisconnectReason '254',
            UNKNOW_ERROR_INSTANCE = 255
        }

        public bool Reconnect(RECONNECT msg)
        {
            if (Account == null)
            {
                string[] labels = new string[] { "{CLIENT_NAME}" };
                string[] arguments = new string[] { Account.Name };

                SendMessage(new FAILURE
                {
                    ErrorId = (int)FailureIDs.JSON_DIALOG,
                    ErrorDescription =
                        JSONErrorIDHandler.
                            FormatedJSONError(
                                errorID: ErrorIDs.LOST_CONNECTION,
                                labels: labels,
                                arguments: arguments
                            )
                });

                Manager.TryDisconnect(this, DisconnectReason.LOST_CONNECTION);

                return false;
            }

            Log.Info($"[({(int)DisconnectReason.RECONNECT}) {DisconnectReason.RECONNECT.ToString()}] Reconnect player '{Account.Name} (Account ID: {Account.AccountId})' to {msg.Name}.");

            Save();

            SendMessage(msg);

            return true;
        }

        public void Save()
        {
            try
            {
                Player?.SaveToCharacter();

                if (Character != null)
                    Manager.Database.SaveCharacter(Account, Character, false);
                if (Account != null)
                    Manager.Database.ReleaseLock(Account);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}