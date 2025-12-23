import { ref } from 'vue';

export class Core {
    name: string;
    status = ref<'available' | 'downloading' | 'downloaded'>('available');
    progress = ref<number>(0);

    constructor(name: string, downloaded: boolean = false) {
        this.name = name;
        if (downloaded) {
            this.status.value = 'downloaded';
        }
    }

    setDownloading() {
        this.status.value = 'downloading';
        this.progress.value = 0;
    }

    setDownloaded() {
        this.status.value = 'downloaded';
        this.progress.value = 100;
    }

    setProgress(progress: number) {
        this.progress.value = progress;
    }

    setError() {
        this.status.value = 'available';
        this.progress.value = 0;
    }
}
