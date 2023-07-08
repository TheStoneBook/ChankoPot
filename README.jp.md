# ChankoPot #

ChankoPotはWindowsの設定を行うWpfアプリケーションである。

## 動作環境
***

* Windows10
* .NET 6.0.15以上
* VisualStudio2022

## 実行方法
***

### Pluginファイル（.dll）作成

ソリューション内のPlugin.Difinition.Basicプロジェクトをビルド。これで、Plugin.Difinition.Basicプロジェクトのバイナリフォルダ内にPlugin.Difinition.Basic.dllが作成される。Plugin.Difinition.Basic.dllはビルド後自動的にChankoPotプロジェクトの実行ファイルと同階層のpluginフォルダ内にコピーされる。

### 実行

実行ファイルを管理者権限で起動。実行ファイルがない場合はChankoPotプロジェクトをビルド。

## 使用方法
***

### UI構成

UIは２段構成になっており、それぞれ以下のような要素を含む。

#### １段目（左から順に）
* Applyボタン
* Saveボタン

#### ２段目（左から順に）
* SettingValueDefinitionエリア
* SettingValueObjectエリア

以下、上記4要素詳細

#### SettingValueDefinitionエリア
アプリケーション起動時に読み込まれたプラグインに含まれるSettingValueDefinitionが並ぶエリア。
#### SettingValueDefinition
値の名称、説明が記載されている。
ドラッグアンドドロップでSettingValueObjectエリアへ移動させるとSettingValueObjectとして、現在の値の取得、変更が行えるようになる。

#### SettingValueObjectエリア
SettingValueObjectが並ぶエリア。
#### SettingValueObject
値の名称、説明、現在の値が記載されている。適用したい値を選択するドロップダウンUIも含まれる。
ドラッグアンドドロップでSettingValueDefinitionエリアへ移動させるとSettingValueDifinitionに戻る。

#### Applyボタン
Windowsへの値適用を行うボタン。値適用にはWindowsの再起動が必要になるため、このボタンをクリックすると、Windowsの再起動を行う旨を記載したメッセージと適用ボタンとキャンセルボタンが表示される。適用ボタンをクリックするとWindowsへの値適用、再起動が行われる。適用されるのはSettingValueObjectエリア内のSettingValueObjectのドロップダウンで選択されている値である。

#### Saveボタン
SettingValueObjectエリア内のSettingValueObjectの設定情報を保存するボタン。
セーブデータはYaml形式で書き出される。

## 新しい設定値の作成方法
***

### SettingDefinitionクラス

本ツールで設定する値を定義したクラス。このクラスを継承することで本ツールで設定できる値を実装することができる。Plugin.Coreプロジェクトに含まれる。

### Plugin.Difinition.Basicプロジェクトを使用する場合

Plugin.Difinition.Basicプロジェクト内にSettingDefinitionクラスを継承した新クラスを作成し、
必要な実装を行う。レジストリを操作したい場合にはRegistrySettingDefinitionクラスを継承する。

### 新プロジェクトを作成する場合

新しいプロジェクトを作成しPlugin.Coreプロジェクトを参照。（作成したプロジェクト上で右クリックし、「ビルドの依存関係/プロジェクトの依存関係」を選択。プロジェクト一覧にあるPlugin.Coreをチェック）新プロジェクト内にSettingDefinitionクラスを継承した新クラスを作成し、必要な実装を行う。