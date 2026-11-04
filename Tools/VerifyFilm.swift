import Foundation
import AVFoundation
let asset=AVURLAsset(url:URL(fileURLWithPath:CommandLine.arguments[1]))
guard let track=asset.tracks(withMediaType:.audio).first else{fatalError("No audio track")}
let reader=try AVAssetReader(asset:asset)
let output=AVAssetReaderTrackOutput(track:track,outputSettings:[AVFormatIDKey:kAudioFormatLinearPCM,AVLinearPCMIsFloatKey:true,AVLinearPCMBitDepthKey:32,AVLinearPCMIsNonInterleaved:false])
reader.add(output);reader.startReading()
var peak:Float=0;var sum:Double=0;var count=0
while let sample=output.copyNextSampleBuffer(){
 guard let block=CMSampleBufferGetDataBuffer(sample) else{continue}
 let bytes=CMBlockBufferGetDataLength(block)
 var values=[Float](repeating:0,count:bytes/4)
 values.withUnsafeMutableBytes{ptr in _=CMBlockBufferCopyDataBytes(block,atOffset:0,dataLength:bytes,destination:ptr.baseAddress!)}
 for v in values {peak=max(peak,abs(v));sum+=Double(v*v);count+=1}
}
guard count>0,peak>0.01,reader.status == .completed else{fatalError("Audio decoding failed or silent")}
let videos=asset.tracks(withMediaType:.video)
let report:[String:Any]=["duration_seconds":CMTimeGetSeconds(asset.duration),"video_tracks":videos.count,"audio_tracks":asset.tracks(withMediaType:.audio).count,"width":videos.first!.naturalSize.width,"height":videos.first!.naturalSize.height,"fps":videos.first!.nominalFrameRate,"decoded_audio_peak_dbfs":20*log10(Double(peak)),"decoded_audio_rms_dbfs":20*log10(sqrt(sum/Double(count))),"decoded_audio_samples":count]
let data=try JSONSerialization.data(withJSONObject:report,options:[.prettyPrinted,.sortedKeys]);print(String(data:data,encoding:.utf8)!)
