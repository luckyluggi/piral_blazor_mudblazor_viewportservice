// import / define the API of your app shell below
type PiletApi = {};
type AddScript = (path: string, attrs?: Record<string, string>) => void;

//import "../../../.piral~/CloudLayout.Ui/dist/_content/MudBlazor/MudBlazor.min.css"
import "../piral~/blazorpilet/dist/_content/MudBlazor/MudBlazor.min.css"

export default function (api: PiletApi, addScript: AddScript) {
    addScript("_content/MudBlazor/MudBlazor.min.js");
}

