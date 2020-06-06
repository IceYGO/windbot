package Cards.CardTypes.Monsters;

public class SpecialSummonableEffectMonster extends EffectMonster{

    public SpecialSummonableEffectMonster(String name, int id) {
        super(name, id);
    }

    @Override
    public String generateAddExecutorMethods() {
        String executors =
                super.generateAddExecutorMethods() +
                this.generateSpSummonExecutor();
        return executors;
    }

    @Override
    public String generateCardMethods() {
        String executors =
                super.generateCardMethods() +
                this.generateSpSummonMethod();
        return executors;
    }
}
