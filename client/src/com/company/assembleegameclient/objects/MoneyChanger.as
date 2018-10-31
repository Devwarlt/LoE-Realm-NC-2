﻿package com.company.assembleegameclient.objects {
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.ui.panels.Panel;

import kabam.rotmg.game.view.MoneyChangerPanel;

public class MoneyChanger extends GameObject implements IInteractiveObject {

    public function MoneyChanger(_arg1:XML) {
        super(_arg1);
        isInteractive_ = true;
        hasShadow_ = false;
    }

    public function getPanel(_arg1:GameSprite):Panel {
        return (new MoneyChangerPanel(_arg1));
    }


}
}
