package Cards.CardTypes;

import Cards.YGOCard;

public class TrapOrSpell extends YGOCard {

    public TrapOrSpell(String name, int id) {
        super(name, id);
    }

    public String generateAddExecutorMethods() {
        String executors =
                this.generateSpellSetExecutor() +
                this.generateActivateExecutor();
        return executors;
    }

    public String generateCardMethods() {
        String methods =
                this.generateSpellSetMethod() +
                this.generateActivateMethod();
        return methods;
    }
}
