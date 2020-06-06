package Cards.CardTypes.Monsters;

import Cards.YGOCard;

public class NormalMonster extends YGOCard {

    public NormalMonster(String name, int id) {
        super(name, id);
    }

    public String generateAddExecutorMethods() {
        String executors =
                this.generateSummonExecutor() +
                this.generateMonsterSetExecutor() +
                this.generateReposExecutor();
        return executors;
    }

    public String generateCardMethods() {
        String methods =
                this.generateSummonMethod() +
                this.generateMonsterSetMethod() +
                this.generateReposMethod();
        return methods;
    }
}
