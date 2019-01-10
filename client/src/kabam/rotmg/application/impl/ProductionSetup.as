package kabam.rotmg.application.impl {
import com.company.assembleegameclient.parameters.Parameters;

import kabam.rotmg.application.api.ApplicationSetup;

public class ProductionSetup implements ApplicationSetup {

    private const SERVER:String = Parameters.ENVIRONMENT_DNS + ":" + Parameters.ENVIRONMENT_PORT;
    private const PROTOCOL:String = "{PROTOCOL}://" + SERVER;
    private const BUILD_LABEL:String = "<font color='#FFFF00'><b>Official</b> {CLIENT_NAME}</font> #{VERSION}.{MINOR}";

    public function getAppEngineUrl(_arg1:Boolean = true):String {
        return this.PROTOCOL.replace("{PROTOCOL}", Parameters.CONNECTION_SECURITY_PROTOCOL);
    }

    public function getBuildLabel():String {
        return this.BUILD_LABEL.replace("{VERSION}", Parameters.BUILD_VERSION).replace("{MINOR}", Parameters.MINOR_VERSION).replace("{CLIENT_NAME}", Parameters.CLIENT_NAME);
    }

    public function useLocalTextures():Boolean {
        return (false);
    }

    public function isToolingEnabled():Boolean {
        return (false);
    }

    public function isGameLoopMonitored():Boolean {
        return (false);
    }

    public function useProductionDialogs():Boolean {
        return (true);
    }

    public function areErrorsReported():Boolean {
        return (false);
    }

    public function areDeveloperHotkeysEnabled():Boolean {
        return (false);
    }

    public function isDebug():Boolean {
        return (false);
    }


}
}
