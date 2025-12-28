import { ref } from 'vue';

export class Game {
    Path: string;
    Name: string;
    SystemName: string;
    CoverName: string;
    CoverData = ref<string>();

    constructor(path: string, name: string, systemName: string, coverName: string) {
        this.Path = path;
        this.Name = name;
        this.SystemName = systemName;
        this.CoverName = coverName;
        this.CoverData.value = undefined;
    }

    setCoverData(data: string | undefined) {
        this.CoverData.value = data;
    }

    getCoverSrc() {
        return this.CoverData.value && `data:image/png;base64,${this.CoverData.value}`;
    }
}