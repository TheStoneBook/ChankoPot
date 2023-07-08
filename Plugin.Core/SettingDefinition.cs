namespace Plugin.Core
{
    public abstract class SettingDefinition
    {
        public abstract string Version { get; }

        /// <summary>
        /// 値の表示名
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 値の適用処理
        /// </summary>
        /// <param name="value"></param>
        public abstract void ApplyValue(string value);

        /// <summary>
        /// 現在の値を取得
        /// </summary>
        /// <returns></returns>
        public abstract string GetValue();

        /// <summary>
        /// ドロップダウンUIに表示される選択肢
        /// </summary>
        public abstract string[] SelectItem { get; }

        /// <summary>
        /// 値の説明。UIに表示される。
        /// </summary>
        public abstract string Description { get; }
    }
}