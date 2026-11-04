import Foundation
import AVFoundation
let args=CommandLine.arguments
let video=AVURLAsset(url:URL(fileURLWithPath:args[1]))
let sound=AVURLAsset(url:URL(fileURLWithPath:args[2]))
let output=URL(fileURLWithPath:args[3])
let composition=AVMutableComposition()
guard let v=video.tracks(withMediaType:.video).first,let a=sound.tracks(withMediaType:.audio).first else {fatalError("Missing source tracks")}
let duration=CMTime(seconds:15,preferredTimescale:600)
let targetV=composition.addMutableTrack(withMediaType:.video,preferredTrackID:kCMPersistentTrackID_Invalid)!
try targetV.insertTimeRange(CMTimeRange(start:.zero,duration:duration),of:v,at:.zero)
targetV.preferredTransform=v.preferredTransform
let targetA=composition.addMutableTrack(withMediaType:.audio,preferredTrackID:kCMPersistentTrackID_Invalid)!
try targetA.insertTimeRange(CMTimeRange(start:.zero,duration:duration),of:a,at:.zero)
if FileManager.default.fileExists(atPath:output.path){try FileManager.default.removeItem(at:output)}
let exporter=AVAssetExportSession(asset:composition,presetName:AVAssetExportPresetHighestQuality)!
exporter.outputURL=output;exporter.outputFileType = .mp4;exporter.shouldOptimizeForNetworkUse=true
let semaphore=DispatchSemaphore(value:0)
exporter.exportAsynchronously{semaphore.signal()};semaphore.wait()
guard exporter.status == .completed else {fatalError(String(describing:exporter.error))}
let result=AVURLAsset(url:output)
print("Verified export: \(CMTimeGetSeconds(result.duration)) seconds, video \(result.tracks(withMediaType:.video).count), audio \(result.tracks(withMediaType:.audio).count)")
