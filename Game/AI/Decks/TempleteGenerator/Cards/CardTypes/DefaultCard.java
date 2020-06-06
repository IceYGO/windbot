package Cards.CardTypes;

import Cards.YGOCard;

public class DefaultCard extends YGOCard {
    public DefaultCard(String name, int id) {
        super(name, id);
    }

    @Override
    public String generateAddExecutorMethods() {
        return "";
    }

    @Override
    public String generateCardMethods() {
        return "";
    }
}
